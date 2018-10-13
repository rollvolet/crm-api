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
    public class BaseInvoiceDataProvider : CaseRelatedDataProvider<DataProvider.Models.Invoice>
    {
        protected readonly ISequenceDataProvider _sequenceDataProvider;
        protected readonly IVatRateDataProvider _vatRateDataProvider;

        public BaseInvoiceDataProvider(ISequenceDataProvider sequenceDataProvider, IVatRateDataProvider vatRateDataProvider,
                                    CrmContext context, IMapper mapper, ILogger<BaseInvoiceDataProvider> logger) : base(context, mapper, logger)
        {
            _sequenceDataProvider = sequenceDataProvider;
            _vatRateDataProvider = vatRateDataProvider;
        }

        protected IQueryable<DataProvider.Models.Invoice> BaseQuery()
        {
            return _context.Invoices;
        }

        protected async Task<DataProvider.Models.Invoice> FindByIdAsync(int id, QuerySet query = null, bool isDepositInvoice = false)
        {
            return await FindWhereAsync(c => c.Id == id, query, isDepositInvoice);
        }

        protected async Task<DataProvider.Models.Invoice> FindWhereAsync(Expression<Func<DataProvider.Models.Invoice, bool>> where,
                                                                          QuerySet query = null, bool isDepositInvoice = false)
        {
            var source = BaseQuery().Where(where);

            if (query != null)
            {
                source = source.Include(query, isDepositInvoice);
                // EF Core doesn't support relationships with a derived type so we have to embed the related resource manually
                return await QueryWithManualIncludeAsync(source, query);
            }
            else
            {
                return await source.FirstOrDefaultAsync();
            }
        }

        protected async Task EmbedCustomerAttributesAsync(DataProvider.Models.Invoice invoice)
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