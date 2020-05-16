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
using Rollvolet.CRM.Domain.Exceptions;
using System;
using System.Linq.Expressions;

namespace Rollvolet.CRM.DataProviders
{
    public class InvoicelineDataProvider : IInvoicelineDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public InvoicelineDataProvider(CrmContext context, IMapper mapper, ILogger<InvoicelineDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Invoiceline> GetByIdAsync(int id, QuerySet query = null)
        {
            var invoiceline = await FindByIdAsync(id, query);

            if (invoiceline == null)
            {
                _logger.LogError($"No invoiceline found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Invoiceline>(invoiceline);
        }

        public async Task<Paged<Invoiceline>> GetAllByOrderIdAsync(int orderId, QuerySet query)
        {
            var source = _context.Invoicelines
                            .Where(o => o.OrderId == orderId)
                            .Include(query)
                            .Sort(query);

            var invoicelines = await source.ForPage(query).ToListAsync();
            var mappedInvoicelines = _mapper.Map<IEnumerable<Invoiceline>>(invoicelines);

            var count = await source.CountAsync();

            return new Paged<Invoiceline>() {
                Items = mappedInvoicelines,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Paged<Invoiceline>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query)
        {
            var source = _context.Invoicelines
                            .Where(o => o.InvoiceId == invoiceId)
                            .Include(query)
                            .Sort(query);

            var invoicelines = await source.ForPage(query).ToListAsync();
            var mappedInvoicelines = _mapper.Map<IEnumerable<Invoiceline>>(invoicelines);

            var count = await source.CountAsync();

            return new Paged<Invoiceline>() {
                Items = mappedInvoicelines,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Invoiceline> CreateAsync(Invoiceline invoiceline)
        {
            var invoicelineRecord = _mapper.Map<DataProvider.Models.Invoiceline>(invoiceline);

            invoicelineRecord.Currency = "EUR";

            _context.Invoicelines.Add(invoicelineRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Invoiceline>(invoicelineRecord);
        }

        public async Task<Invoiceline> UpdateAsync(Invoiceline invoiceline)
        {
            var invoicelineRecord = await FindByIdAsync(invoiceline.Id);
            _mapper.Map(invoiceline, invoicelineRecord);

            invoicelineRecord.Currency = "EUR";

            _context.Invoicelines.Update(invoicelineRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Invoiceline>(invoicelineRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var invoiceline = await FindByIdAsync(id);

            if (invoiceline != null)
            {
                _context.Invoicelines.Remove(invoiceline);
                await _context.SaveChangesAsync();
           }
        }

        private async Task<DataProvider.Models.Invoiceline> FindByIdAsync(int id, QuerySet query = null)
        {
            return await FindWhereAsync(c => c.Id == id, query);
        }

        private async Task<DataProvider.Models.Invoiceline> FindWhereAsync(Expression<Func<DataProvider.Models.Invoiceline, bool>> where, QuerySet query = null)
        {
            var source = _context.Invoicelines.Where(where);

            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }
    }
}