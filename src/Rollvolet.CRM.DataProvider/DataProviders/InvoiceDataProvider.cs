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
using LinqKit;
using System;
using Rollvolet.CRM.Domain.Exceptions;
using System.Linq.Expressions;

namespace Rollvolet.CRM.DataProviders
{
    public class InvoiceDataProvider : BaseInvoiceDataProvider, IInvoiceDataProvider
    {
        private readonly IInvoiceSupplementDataProvider _invoiceSupplementDataProvider;

        public InvoiceDataProvider(IInvoiceSupplementDataProvider invoiceSupplementDataProvider,
                                    ISequenceDataProvider sequenceDataProvider, IVatRateDataProvider vatRateDataProvider,
                                    CrmContext context, IMapper mapper, ILogger<InvoiceDataProvider> logger)
                                    : base(sequenceDataProvider, vatRateDataProvider, context, mapper, logger)
        {
            _invoiceSupplementDataProvider = invoiceSupplementDataProvider;
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

            // EF Core doesn't support relationships with a derived type so we have to embed the related resource manually
            var invoices = QueryListWithManualInclude(source, query);

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
            var source = BaseQuery()
                            .Where(o => o.CustomerId == customerId)
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

            var invoices = QueryListWithManualInclude(source, query);

            var mappedInvoices = _mapper.Map<IEnumerable<Invoice>>(invoices);

            var count = await source.CountAsync();

            return new Paged<Invoice>() {
                Items = mappedInvoices,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
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

            invoiceRecord.Number = await _sequenceDataProvider.GetNextInvoiceNumber();
            invoiceRecord.Currency = "EUR";
            invoiceRecord.Year = (short) DateTime.Now.Year;

            await EmbedCustomerAttributesAsync(invoiceRecord);
            await CalculateAmountAndVatAsync(invoiceRecord);

            _context.Invoices.Add(invoiceRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Invoice>(invoiceRecord);
        }

        public async Task<Invoice> UpdateAsync(Invoice invoice)
        {
            var invoiceRecord = await FindByIdAsync(invoice.Id);

            // compare old and new attribute values before merging the changes in the invoiceRecord
            var requiresRecalculation = invoice.BaseAmount != invoiceRecord.BaseAmount || int.Parse(invoice.VatRate.Id) != invoiceRecord.VatRateId;

            _mapper.Map(invoice, invoiceRecord);

            invoiceRecord.Currency = "EUR";
            await EmbedCustomerAttributesAsync(invoiceRecord);

            // Only a change of the baseAmount or vatRate trigger a recalculation.
            // Updates by a change in other resources (e.g. invoice supplements) are triggered through SyncAmountAndVatAsync
            if (requiresRecalculation)
                await CalculateAmountAndVatAsync(invoiceRecord);

            _context.Invoices.Update(invoiceRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Invoice>(invoiceRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var invoice = await FindByIdAsync(id);

            if (invoice != null)
            {
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

        private async Task CalculateAmountAndVatAsync(DataProvider.Models.Invoice invoice)
        {
            _logger.LogDebug("Recalculating amount and VAT of invoice {0}", invoice.Id);
            var query = new QuerySet();
            var invoiceSupplements = await _invoiceSupplementDataProvider.GetAllByInvoiceIdAsync(invoice.Id, query);
            var invoiceSupplementsTotal = invoiceSupplements.Items.Select(s => s.Total).Sum() ?? 0.0;
            var baseAmount = invoice.BaseAmount ?? 0.0;
            var amount = baseAmount + invoiceSupplementsTotal; // TODO minus all deposit invoices

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
        }
    }
}
