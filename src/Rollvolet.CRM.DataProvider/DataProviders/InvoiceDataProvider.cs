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

namespace Rollvolet.CRM.DataProviders
{   
    public class InvoiceDataProvider : CaseRelatedDataProvider<DataProvider.Models.Invoice>, IInvoiceDataProvider
    {

        public InvoiceDataProvider(CrmContext context, IMapper mapper, ILogger<InvoiceDataProvider> logger) : base(context, mapper, logger)
        {

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

        public async Task<Invoice> GetByIdAsync(int id, QuerySet query)
        {
            var source = BaseQuery()
                            .Where(c => c.Id == id)
                            .Include(query);

            // EF Core doesn't support relationships with a derived type so we have to embed the related resource manually
            var invoice = await QueryWithManualIncludeAsync(source, query);

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
    }
}