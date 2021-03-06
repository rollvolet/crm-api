using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.DataProvider.Extensions;
using Microsoft.Extensions.Logging;
using System;
using Rollvolet.CRM.Domain.Exceptions;
using System.Linq.Expressions;
using Z.EntityFramework.Plus;

namespace Rollvolet.CRM.DataProviders
{
    public class InvoiceDataProvider : BaseInvoiceDataProvider, IInvoiceDataProvider
    {
        private readonly IInvoiceSupplementDataProvider _invoiceSupplementDataProvider;
        private readonly IDepositInvoiceDataProvider _depositInvoiceDataProvider;
        private readonly IInvoicelineDataProvider _invoicelineDataProvider;

        public InvoiceDataProvider(IInvoiceSupplementDataProvider invoiceSupplementDataProvider,
                                    IDepositInvoiceDataProvider depositInvoiceDataProvider,
                                    IInvoicelineDataProvider invoicelineDataProvider,
                                    ISequenceDataProvider sequenceDataProvider, IVatRateDataProvider vatRateDataProvider,
                                    CrmContext context, IMapper mapper, ILogger<InvoiceDataProvider> logger)
                                    : base(sequenceDataProvider, vatRateDataProvider, context, mapper, logger)
        {
            _invoiceSupplementDataProvider = invoiceSupplementDataProvider;
            _depositInvoiceDataProvider = depositInvoiceDataProvider;
            _invoicelineDataProvider = invoicelineDataProvider;
        }

        private new IQueryable<DataProvider.Models.Invoice> BaseQuery() {
            return _context.Invoices
                            .Where(i => i.MainInvoiceHub == null); // exclude deposit invoices
        }

        public async Task<Paged<Invoice>> GetAllAsync(QuerySet query)
        {
            var source = BaseQuery()
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

            var invoices = await source.ForPage(query).ToListAsync();

            var mappedInvoices = _mapper.Map<IEnumerable<Invoice>>(invoices);

            var count = await source.CountAsync();

            return new Paged<Invoice>() {
                Items = mappedInvoices,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Invoice> GetByIdAsync(int id, QuerySet query = null)
        {
            var invoice = await FindByIdAsync(id, query);

            if (invoice == null)
            {
                _logger.LogError($"No invoice found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Invoice>(invoice);
        }

        public async Task<Paged<Invoice>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            return await GetAllWhereAsync(o => o.CustomerId == customerId, query);
        }

        public async Task<Paged<Invoice>> GetAllByRelativeContactIdAsync(int customerId, int relativeContactId, QuerySet query)
        {
            return await GetAllWhereAsync(r => r.CustomerId == customerId && r.RelativeContactId == relativeContactId, query);
        }

        public async Task<Paged<Invoice>> GetAllByRelativeBuildingIdAsync(int customerId, int relativeBuildingId, QuerySet query)
        {
            return await GetAllWhereAsync(r => r.CustomerId == customerId && r.RelativeBuildingId == relativeBuildingId, query);
        }

        public async Task<Invoice> GetByOrderIdAsync(int orderId, QuerySet query = null)
        {
            var invoice = await FindWhereAsync(i => i.OrderId == orderId, query);

            if (invoice == null)
            {
                _logger.LogError($"No invoice found for order-id {orderId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Invoice>(invoice);
        }

        public async Task<Invoice> GetByInterventionIdAsync(int interventionId, QuerySet query = null)
        {
            var invoice = await FindWhereAsync(i => i.InterventionId == interventionId, query);

            if (invoice == null)
            {
                _logger.LogError($"No invoice found for intervention-id {interventionId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Invoice>(invoice);
        }

        public async Task<Invoice> GetByInvoicelineIdAsync(int invoicelineId)
        {
            var invoice = await _context.Invoicelines.Where(o => o.Id == invoicelineId).Select(o => o.Invoice).FirstOrDefaultAsync();

            if (invoice == null)
            {
                _logger.LogError($"No invoice found for invoiceline-id {invoicelineId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Invoice>(invoice);
        }

        public async Task<Invoice> GetByWorkingHourIdAsync(int workingHourId)
        {
            var invoice = await _context.WorkingHours.Where(w => w.Id == workingHourId).Select(w => w.Invoice).FirstOrDefaultAsync();

            if (invoice == null)
            {
                _logger.LogError($"No invoice found for working-hour-id {workingHourId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Invoice>(invoice);
        }

        public async Task<Invoice> CreateAsync(Invoice invoice)
        {
            var invoiceRecord = _mapper.Map<DataProvider.Models.Invoice>(invoice);

            invoiceRecord.Number = await _sequenceDataProvider.GetNextInvoiceNumberAsync();
            invoiceRecord.Currency = "EUR";
            invoiceRecord.Year = (short) DateTimeOffset.UtcNow.UtcDateTime.Year;

            await EmbedCustomerAttributesAsync(invoiceRecord);
            await CalculateAmountAndVatAsync(invoiceRecord);

            _context.Invoices.Add(invoiceRecord);
            await _context.SaveChangesAsync();

            if (invoiceRecord.OrderId != null)
            {
                // Attach all deposits and deposit invoices of the order to the new invoice
                var depositInvoices = await _context.DepositInvoices.Where(hub => hub.OrderId == invoiceRecord.OrderId).ToListAsync();
                foreach (var depositInvoice in depositInvoices)
                {
                    depositInvoice.InvoiceId = invoiceRecord.Id;
                }
                var deposits = await _context.Deposits.Where(d => d.OrderId == invoiceRecord.OrderId).ToListAsync();
                foreach (var deposit in deposits)
                {
                    deposit.InvoiceId = invoiceRecord.Id;
                }
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<Invoice>(invoiceRecord);
        }

        public async Task<Invoice> UpdateAsync(Invoice invoice)
        {
            var invoiceRecord = await FindByIdAsync(invoice.Id);

            _mapper.Map(invoice, invoiceRecord);

            invoiceRecord.Currency = "EUR";
            await EmbedCustomerAttributesAsync(invoiceRecord);

            // Updates by a change in other resources (e.g. invoice supplements, invoicelines) are triggered through SyncAmountAndVatAsync
            await CalculateAmountAndVatAsync(invoiceRecord);

            _context.Invoices.Update(invoiceRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Invoice>(invoiceRecord);
        }

        public async Task<Invoice> UpdateContactAndBuildingAsync(int id, int? relativeContactId, int? relativeBuildingId)
        {
            var invoiceRecord = await FindByIdAsync(id);
            invoiceRecord.RelativeContactId = relativeContactId;
            invoiceRecord.RelativeBuildingId = relativeBuildingId;

            await EmbedCustomerAttributesAsync(invoiceRecord);

            _context.Invoices.Update(invoiceRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Invoice>(invoiceRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var invoice = await FindByIdAsync(id);

            if (invoice != null)
            {
                if (invoice.OrderId != null)
                {
                    // Detach all deposits and deposit invoices. They are still attached to the order
                    var depositInvoices = await _context.DepositInvoices.Where(hub => hub.OrderId == invoice.OrderId).ToListAsync();
                    foreach (var depositInvoice in depositInvoices)
                    {
                        depositInvoice.InvoiceId = null;
                    }
                    var deposits = await _context.Deposits.Where(d => d.OrderId == invoice.OrderId).ToListAsync();
                    foreach (var deposit in deposits)
                    {
                        deposit.InvoiceId = null;
                    }
                }

                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
           }
        }

        public async Task SyncAmountAndVatAsync(int id)
        {
            _logger.LogDebug("Recalculation of amount and VAT of invoice {0} triggered", id);

            var invoiceRecord = await FindByIdAsync(id);

            if (invoiceRecord != null)
            {
                await CalculateAmountAndVatAsync(invoiceRecord);
                _context.Invoices.Update(invoiceRecord);
                await _context.SaveChangesAsync();
                _logger.LogDebug("Successfully recalculated amount and VAT of invoice {0}", id);
            }
        }

        private async Task<Paged<Invoice>> GetAllWhereAsync(Expression<Func<DataProvider.Models.Invoice, bool>> where, QuerySet query)
        {
            var source = BaseQuery()
                            .Where(where)
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

            var invoices = await source.ForPage(query).ToListAsync();

            var mappedInvoices = _mapper.Map<IEnumerable<Invoice>>(invoices);

            var count = await source.CountAsync();

            return new Paged<Invoice>() {
                Items = mappedInvoices,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        private async Task CalculateAmountAndVatAsync(DataProvider.Models.Invoice invoice)
        {
            _logger.LogDebug("Recalculating amount and VAT of invoice {0}", invoice.Id);

            var depositInvoicesTotal = 0.0;
            if (invoice.OrderId != null)
            {
                var query = new QuerySet();
                query.Page.Size = 1000; // TODO we assume 1 order doesn't have more than 1000 deposit invoices. Ideally, we should query by page.
                // don't GetByInvoiceId because invoice might not be persisted yet
                var depositInvoices = await _depositInvoiceDataProvider.GetAllByOrderIdAsync((int) invoice.OrderId, query);
                depositInvoicesTotal = depositInvoices.Items.Select(s => s.IsCreditNote ? s.Amount * -1.0 : s.Amount).Sum() ?? 0.0;
            }

            var invoiceSupplementsTotal = 0.0;
            if (invoice.Id != 0)  // invoice supplements can only be attached to an existing invoice
            {
                var query = new QuerySet();
                query.Page.Size = 1000; // TODO we assume 1 invoice doesn't have more than 1000 supplements. Ideally, we should query by page.
                var invoiceSupplements = await _invoiceSupplementDataProvider.GetAllByInvoiceIdAsync(invoice.Id, query);
                invoiceSupplementsTotal = invoiceSupplements.Items.Select(s => s.Amount).Sum() ?? 0.0;
            }

            var invoicelinesTotal = 0.0;
            if (invoice.Id != 0)  // invoice supplements can only be attached to an existing invoice
            {
                var query = new QuerySet();
                query.Page.Size = 10000; // TODO we assume 1 invoice doesn't have more than 10000 lines. Ideally, we should query by page.
                var invoicelines = await _invoicelineDataProvider.GetAllByInvoiceIdAsync(invoice.Id, query);
                invoicelinesTotal = invoicelines.Items.Select(s => s.Amount).Sum() ?? 0.0;
            }

            var baseAmount = invoice.BaseAmount ?? 0.0;
            var amount = baseAmount + invoiceSupplementsTotal + invoicelinesTotal - depositInvoicesTotal;

            var vat = 0.0;
            if (invoice.VatRateId != null)
            {
                // don't GetByInvoiceId because invoice might not be persisted yet
                var vatRate = await _vatRateDataProvider.GetByIdAsync((int) invoice.VatRateId);
                vat = amount * (vatRate.Rate / 100.0);
            }

            invoice.Amount = amount; // sum of base amount + all invoice supplements - all deposit invoices
            invoice.Vat = vat; // vat calculated on amount
            invoice.TotalAmount = amount + vat; // gross amount

            _logger.LogDebug("Recalculated amount of invoice {0}: amount {1}} ; vat {2} ; total amount {3}", invoice.Id, invoice.Amount, invoice.Vat, invoice.TotalAmount);
        }
    }
}
