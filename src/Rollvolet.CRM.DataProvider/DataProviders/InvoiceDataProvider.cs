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
    public class InvoiceDataProvider : CaseRelatedDataProvider<DataProvider.Models.Invoice>, IInvoiceDataProvider
    {
        private readonly ISequenceDataProvider _sequenceDataProvider;
        private readonly IInvoiceSupplementDataProvider _invoiceSupplementDataProvider;
        private readonly IVatRateDataProvider _vatRateDataProvider;

        public InvoiceDataProvider(ISequenceDataProvider sequenceDataProvider, IInvoiceSupplementDataProvider invoiceSupplementDataProvider,
                                    IVatRateDataProvider vatRateDataProvider,
                                    CrmContext context, IMapper mapper, ILogger<InvoiceDataProvider> logger) : base(context, mapper, logger)
        {
            _sequenceDataProvider = sequenceDataProvider;
            _invoiceSupplementDataProvider = invoiceSupplementDataProvider;
            _vatRateDataProvider = vatRateDataProvider;
        }

        private IQueryable<DataProvider.Models.Invoice> BaseQuery() {
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
            invoiceRecord.Year = (short) new DateTime().Year;

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

        private async Task<DataProvider.Models.Invoice> FindByIdAsync(int id, QuerySet query = null)
        {
            return await FindWhereAsync(c => c.Id == id, query);
        }

        private async Task<DataProvider.Models.Invoice> FindWhereAsync(Expression<Func<DataProvider.Models.Invoice, bool>> where, QuerySet query = null)
        {
            var source = _context.Invoices.Where(where);

            if (query != null)
            {
                source = source.Include(query);
                // EF Core doesn't support relationships with a derived type so we have to embed the related resource manually
                return await QueryWithManualIncludeAsync(source, query);
            }
            else
            {
                return await source.FirstOrDefaultAsync();
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

        private async Task EmbedCustomerAttributesAsync(DataProvider.Models.Invoice invoice)
        {
            var customer = await _context.Customers.Where(c => c.Number == invoice.CustomerId).FirstOrDefaultAsync();
            invoice.CustomerName = customer.Name;
            invoice.CustomerAddress1 = customer.Address1;
            invoice.CustomerAddress2 = customer.Address2;
            invoice.CustomerAddress3 = customer.Address3;
            invoice.CustomerPostalCodeId = customer.PostalCodeId;
            invoice.CustomerPostalCode = customer.EmbeddedPostalCode;
            invoice.CustomerCity = customer.EmbeddedCity;
            invoice.CustomerLanguageId = customer.LanguageId;
            invoice.CustomerCountryId = customer.CountryId;
            invoice.CustomerHonorificPrefixId = customer.HonorificPrefixId;
            invoice.CustomerPrefix = customer.Prefix;
            invoice.CustomerSuffix = customer.Suffix;
            invoice.CustomerIsCompany = customer.IsCompany;
            invoice.CustomerVatNumber = customer.VatNumber;
            // TODO embed phone, mobile and fax number
            invoice.CustomerSearchName = customer.SearchName;

            if (invoice.RelativeBuildingId != null)
            {
                var building = await _context.Buildings.
                                        Where(b => b.CustomerId == invoice.CustomerId && b.Number == invoice.RelativeBuildingId).FirstOrDefaultAsync();
                invoice.BuildingName = building.Name;
                invoice.BuildingAddress1 = building.Address1;
                invoice.BuildingAddress2 = building.Address2;
                invoice.BuildingAddress3 = building.Address3;
                invoice.BuildingPostalCodeId = building.PostalCodeId;
                invoice.BuildingPostalCode = building.EmbeddedPostalCode;
                invoice.BuildingCity = building.EmbeddedCity;
                invoice.BuildingCountryId = building.CountryId;
                invoice.BuildingPrefix = building.Prefix;
                invoice.BuildingSuffix = building.Suffix;
                // TODO embed phone, mobile and fax number
                invoice.BuildingSearchName = building.SearchName;
            }

            if (invoice.RelativeContactId != null)
            {
                var contact = await _context.Contacts.
                                        Where(b => b.CustomerId == invoice.CustomerId && b.Number == invoice.RelativeContactId).FirstOrDefaultAsync();
                invoice.ContactName = contact.Name;
                invoice.ContactAddress1 = contact.Address1;
                invoice.ContactAddress2 = contact.Address2;
                invoice.ContactAddress3 = contact.Address3;
                invoice.ContactPostalCodeId = contact.PostalCodeId;
                invoice.ContactPostalCode = contact.EmbeddedPostalCode;
                invoice.ContactCity = contact.EmbeddedCity;
                invoice.ContactLanguageId = contact.LanguageId;
                invoice.ContactCountryId = contact.CountryId;
                invoice.ContactHonorificPrefixId = contact.HonorificPrefixId;
                invoice.ContactPrefix = contact.Prefix;
                invoice.ContactSuffix = contact.Suffix;
                // TODO embed phone, mobile and fax number
                invoice.ContactSearchName = contact.SearchName;
            }

            if (invoice.BuildingCountryId == null)
                invoice.BuildingCountryId = customer.CountryId; // not-NULL DB contstraint
            if (invoice.ContactCountryId == null)
                invoice.ContactCountryId = customer.CountryId; // not-NULL DB contstraint
        }
    }
}
