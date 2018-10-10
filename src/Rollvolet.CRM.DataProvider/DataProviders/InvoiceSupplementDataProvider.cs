using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Extensions;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProviders
{
    public class InvoiceSupplementDataProvider : IInvoiceSupplementDataProvider
    {
        protected readonly CrmContext _context;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;

        public InvoiceSupplementDataProvider(CrmContext context, IMapper mapper, ILogger<RequestDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Paged<InvoiceSupplement>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query)
        {
            var source = _context.InvoiceSupplements
                            .Where(s => s.InvoiceId == invoiceId)
                            .Include(query)
                            .Sort(query)
                            .Filter(query);


            var invoiceSupplements = source.ForPage(query).AsEnumerable();

            var count = await source.CountAsync();

            var mappedInvoiceSupplements = _mapper.Map<IEnumerable<InvoiceSupplement>>(invoiceSupplements);

            return new Paged<InvoiceSupplement>() {
                Items = mappedInvoiceSupplements,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public Task<InvoiceSupplement> CreateAsync(InvoiceSupplement invoiceSupplement)
        {
            throw new System.NotImplementedException();
        }

        public Task<InvoiceSupplement> UpdateAsync(InvoiceSupplement invoiceSupplement)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }
  }
}