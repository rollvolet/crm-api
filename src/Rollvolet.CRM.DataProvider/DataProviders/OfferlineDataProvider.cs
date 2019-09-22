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
    public class OfferlineDataProvider : IOfferlineDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public OfferlineDataProvider(CrmContext context, IMapper mapper, ILogger<DepositDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Offerline> GetByIdAsync(int id, QuerySet query = null)
        {
            var offerline = await FindByIdAsync(id, query);

            if (offerline == null)
            {
                _logger.LogError($"No offerline found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Offerline>(offerline);
        }

        public async Task<Paged<Offerline>> GetAllByOfferIdAsync(int offerId, QuerySet query)
        {
            var source = _context.Offerlines
                            .Where(o => o.OfferId == offerId)
                            .Include(query)
                            .Sort(query);

            var offerlines = source.ForPage(query).AsEnumerable();
            var mappedOfferlines = _mapper.Map<IEnumerable<Offerline>>(offerlines);

            var count = await source.CountAsync();

            return new Paged<Offerline>() {
                Items = mappedOfferlines,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Paged<Offerline>> GetOrderedByOrderIdAsync(int orderId, QuerySet query)
        {
            var source = _context.Offerlines
                            .Where(o => o.OfferId == orderId && o.IsOrdered)
                            .Include(query)
                            .Sort(query);

            var offerlines = source.ForPage(query).AsEnumerable();
            var mappedOfferlines = _mapper.Map<IEnumerable<Offerline>>(offerlines);

            var count = await source.CountAsync();

            return new Paged<Offerline>() {
                Items = mappedOfferlines,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Offerline> CreateAsync(Offerline offerline)
        {
            var offerlineRecord = _mapper.Map<DataProvider.Models.Offerline>(offerline);

            offerlineRecord.Currency = "EUR";

            _context.Offerlines.Add(offerlineRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Offerline>(offerlineRecord);
        }

        public async Task<Offerline> UpdateAsync(Offerline offerline)
        {
            var offerlineRecord = await FindByIdAsync(offerline.Id);
            _mapper.Map(offerline, offerlineRecord);

            offerlineRecord.Currency = "EUR";

            _context.Offerlines.Update(offerlineRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Offerline>(offerlineRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var offerline = await FindByIdAsync(id);

            if (offerline != null)
            {
                _context.Offerlines.Remove(offerline);
                await _context.SaveChangesAsync();
           }
        }

        private async Task<DataProvider.Models.Offerline> FindByIdAsync(int id, QuerySet query = null)
        {
            return await FindWhereAsync(c => c.Id == id, query);
        }

        private async Task<DataProvider.Models.Offerline> FindWhereAsync(Expression<Func<DataProvider.Models.Offerline, bool>> where, QuerySet query = null)
        {
            var source = _context.Offerlines.Where(where);

            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }
    }
}