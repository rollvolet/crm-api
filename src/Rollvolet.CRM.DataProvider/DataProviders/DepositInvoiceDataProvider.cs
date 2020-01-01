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
using Rollvolet.CRM.DataProvider.Models;

namespace Rollvolet.CRM.DataProviders
{
    public class DepositInvoiceDataProvider : BaseInvoiceDataProvider, IDepositInvoiceDataProvider
    {
        public DepositInvoiceDataProvider(ISequenceDataProvider sequenceDataProvider, IVatRateDataProvider vatRateDataProvider,
                                            CrmContext context, IMapper mapper, ILogger<DepositInvoiceDataProvider> logger)
                                            : base(sequenceDataProvider, vatRateDataProvider, context, mapper, logger)
        {

        }

        private new IQueryable<DataProvider.Models.Invoice> BaseQuery() {
            return _context.Invoices
                            .Where(i => i.MainInvoiceHub != null); // only deposit invoices
        }


        public async Task<Paged<DepositInvoice>> GetAllAsync(QuerySet query)
        {
            var source = BaseQuery()
                            .Include(query, true)
                            .Sort(query, true)
                            .Filter(query, _context, true);

            // EF Core doesn't support relationships with a derived type so we have to embed the related resource manually
            var invoices = QueryListWithManualInclude(source, query);

            var mappedInvoices = _mapper.Map<IEnumerable<DepositInvoice>>(invoices);

            var count = await source.CountAsync();

            return new Paged<DepositInvoice>() {
                Items = mappedInvoices,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<DepositInvoice> GetByIdAsync(int id, QuerySet query = null)
        {
            var invoice = await FindByIdAsync(id, query, true);

            if (invoice == null)
            {
                _logger.LogError($"No deposit invoice found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<DepositInvoice>(invoice);
        }

        public async Task<Paged<DepositInvoice>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            var source = BaseQuery()
                            .Where(o => o.CustomerId == customerId)
                            .Include(query, true)
                            .Sort(query, true)
                            .Filter(query, _context, true);

            var invoices = QueryListWithManualInclude(source, query);

            var mappedInvoices = _mapper.Map<IEnumerable<DepositInvoice>>(invoices);

            var count = await source.CountAsync();

            return new Paged<DepositInvoice>() {
                Items = mappedInvoices,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Paged<DepositInvoice>> GetAllByOrderIdAsync(int orderId, QuerySet query)
        {
            var source = BaseQuery()
                            .Where(i => i.MainInvoiceHub.OrderId == orderId)
                            .Include(query, true)
                            .Sort(query, true)
                            .Filter(query, _context, true);

            // EF Core doesn't support relationships with a derived type so we have to embed the related resource manually
            var invoices = QueryListWithManualInclude(source, query);

            var mappedInvoices = _mapper.Map<IEnumerable<DepositInvoice>>(invoices);

            var count = await source.CountAsync();

            return new Paged<DepositInvoice>() {
                Items = mappedInvoices,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Paged<DepositInvoice>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query)
        {
            var source = BaseQuery()
                            .Where(i => i.MainInvoiceHub.InvoiceId == invoiceId)
                            .Include(query, true)
                            .Sort(query, true)
                            .Filter(query, _context, true);

            // EF Core doesn't support relationships with a derived type so we have to embed the related resource manually
            var invoices = QueryListWithManualInclude(source, query);

            var mappedInvoices = _mapper.Map<IEnumerable<DepositInvoice>>(invoices);

            var count = await source.CountAsync();

            return new Paged<DepositInvoice>() {
                Items = mappedInvoices,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<DepositInvoice> CreateAsync(DepositInvoice depositInvoice)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var depositInvoiceRecord = _mapper.Map<DataProvider.Models.Invoice>(depositInvoice);

                    depositInvoiceRecord.Number = await _sequenceDataProvider.GetNextInvoiceNumberAsync();
                    depositInvoiceRecord.Currency = "EUR";
                    depositInvoiceRecord.Year = (short) DateTime.Now.Year;

                    await EmbedCustomerAttributesAsync(depositInvoiceRecord);
                    await CalculateAmountAndVatAsync(depositInvoiceRecord);

                    _context.Invoices.Add(depositInvoiceRecord);
                    await _context.SaveChangesAsync();

                    var depositInvoiceHub = new DepositInvoiceHub();
                    depositInvoiceHub.CustomerId = depositInvoice.Customer.Id;
                    depositInvoiceHub.OrderId = depositInvoice.Order.Id;
                    depositInvoiceHub.Date = DateTime.Now;
                    depositInvoiceHub.DepositInvoiceId = depositInvoiceRecord.Id;

                    var invoice = await _context.Orders.Where(o => o.Id == depositInvoice.Order.Id).Select(o => o.Invoice).FirstOrDefaultAsync();
                    if (invoice != null)
                        depositInvoiceHub.InvoiceId = invoice.Id;

                    _context.DepositInvoices.Add(depositInvoiceHub);
                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return _mapper.Map<DepositInvoice>(depositInvoiceRecord);

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public async Task<DepositInvoice> UpdateAsync(DepositInvoice depositInvoice)
        {
            var depositInvoiceRecord = await FindByIdAsync(depositInvoice.Id);

            _mapper.Map(depositInvoice, depositInvoiceRecord);

            await EmbedCustomerAttributesAsync(depositInvoiceRecord);
            await CalculateAmountAndVatAsync(depositInvoiceRecord);

            _context.Invoices.Update(depositInvoiceRecord);
            await _context.SaveChangesAsync();

            // Deposit invoice hub doesn't need to be updated since none of the attributes can change

            return _mapper.Map<DepositInvoice>(depositInvoiceRecord);
        }

        public async Task<DepositInvoice> UpdateContactAndBuildingAsync(int id, int? relativeContactId, int? relativeBuildingId)
        {
            var depositInvoiceRecord = await FindByIdAsync(id);
            depositInvoiceRecord.RelativeContactId = relativeContactId;
            depositInvoiceRecord.RelativeBuildingId = relativeBuildingId;

            await EmbedCustomerAttributesAsync(depositInvoiceRecord);

            _context.Invoices.Update(depositInvoiceRecord);
            await _context.SaveChangesAsync();

            // Deposit invoice hub doesn't need to be updated since none of the attributes changes

            return _mapper.Map<DepositInvoice>(depositInvoiceRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var depositInvoiceHub = await _context.DepositInvoices.Where(h => h.DepositInvoiceId == id).FirstOrDefaultAsync();

            if (depositInvoiceHub != null)
                _context.DepositInvoices.Remove(depositInvoiceHub);

            var invoice = await FindByIdAsync(id);

            if (invoice != null)
                _context.Invoices.Remove(invoice);

            await _context.SaveChangesAsync();
        }

        private async Task CalculateAmountAndVatAsync(DataProvider.Models.Invoice depositInvoice)
        {
            _logger.LogDebug("Recalculating amount and VAT of deposit invoice {0}", depositInvoice.Id);
            var amount = depositInvoice.BaseAmount ?? 0.0;

            var vat = 0.0;
            if (depositInvoice.VatRateId != null)
            {
                // don't GetByInvoiceId because invoice might not be persisted yet
                var vatRate = await _vatRateDataProvider.GetByIdAsync((int) depositInvoice.VatRateId);
                vat = amount * (vatRate.Rate / 100.0);
            }

            depositInvoice.Amount = amount; // base amount
            depositInvoice.Vat = vat; // vat calculated on amount
            depositInvoice.TotalAmount = amount + vat; // gross amount
        }
    }
}