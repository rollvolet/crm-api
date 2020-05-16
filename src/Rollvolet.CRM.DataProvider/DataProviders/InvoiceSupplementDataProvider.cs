using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Extensions;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
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

        public async Task<InvoiceSupplement> GetByIdAsync(int id, QuerySet query = null)
        {
            var invoiceSupplement = await FindByIdAsync(id, query);

            if (invoiceSupplement == null)
            {
                _logger.LogError($"No invoice-supplement found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<InvoiceSupplement>(invoiceSupplement);
        }

        public async Task<Paged<InvoiceSupplement>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query)
        {
            var source = _context.InvoiceSupplements
                            .Where(s => s.InvoiceId == invoiceId)
                            .Include(query)
                            .Sort(query)
                            .Filter(query)
                            .ForPage(query);

            var invoiceSupplements = await source.ToListAsync();

            var count = await source.CountAsync();

            var mappedInvoiceSupplements = _mapper.Map<IEnumerable<InvoiceSupplement>>(invoiceSupplements);

            return new Paged<InvoiceSupplement>() {
                Items = mappedInvoiceSupplements,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<InvoiceSupplement> CreateAsync(InvoiceSupplement invoiceSupplement)
        {
            var invoiceSupplementRecord = _mapper.Map<DataProvider.Models.InvoiceSupplement>(invoiceSupplement);

            invoiceSupplementRecord.Currency = "EUR";
            invoiceSupplementRecord.SequenceNumber = 0;

            _context.InvoiceSupplements.Add(invoiceSupplementRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<InvoiceSupplement>(invoiceSupplementRecord);
        }

        public async Task<InvoiceSupplement> UpdateAsync(InvoiceSupplement invoiceSupplement)
        {
            var invoiceSupplementRecord = await FindByIdAsync(invoiceSupplement.Id);
            _mapper.Map(invoiceSupplement, invoiceSupplementRecord);

            invoiceSupplementRecord.Currency = "EUR";
            invoiceSupplementRecord.SequenceNumber = 0;

            _context.InvoiceSupplements.Update(invoiceSupplementRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<InvoiceSupplement>(invoiceSupplementRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var invoiceSupplement = await FindByIdAsync(id);

            if (invoiceSupplement != null)
            {
                _context.InvoiceSupplements.Remove(invoiceSupplement);
                await _context.SaveChangesAsync();
           }
        }

        private async Task<DataProvider.Models.InvoiceSupplement> FindByIdAsync(int id, QuerySet query = null)
        {
            return await FindWhereAsync(c => c.Id == id, query);
        }

        private async Task<DataProvider.Models.InvoiceSupplement> FindWhereAsync(Expression<Func<DataProvider.Models.InvoiceSupplement, bool>> where, QuerySet query = null)
        {
            var source = _context.InvoiceSupplements.Where(where);

            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }
    }
}