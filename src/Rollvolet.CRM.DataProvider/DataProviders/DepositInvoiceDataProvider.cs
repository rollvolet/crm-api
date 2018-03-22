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

namespace Rollvolet.CRM.DataProviders
{   
    public class DepositInvoiceDataProvider : CaseRelatedDataProvider<DataProvider.Models.Invoice>, IDepositInvoiceDataProvider
    {
        public DepositInvoiceDataProvider(CrmContext context, IMapper mapper, ILogger<InvoiceDataProvider> logger) : base(context, mapper, logger)
        {

        }

        private IQueryable<DataProvider.Models.Invoice> BaseQuery() {
            return _context.Invoices
                            .Where(i => i.MainInvoiceHub != null); // only deposit invoices
        }

        public async Task<Paged<DepositInvoice>> GetAllByOrderIdAsync(int orderId, QuerySet query)
        {
            var source = BaseQuery()
                            .Where(i => i.MainInvoiceHub.OrderId == orderId)
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

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
    }
}