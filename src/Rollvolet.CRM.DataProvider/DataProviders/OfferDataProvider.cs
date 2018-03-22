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
using Rollvolet.CRM.Domain.Exceptions;

namespace Rollvolet.CRM.DataProviders
{   
    public class OfferDataProvider : CaseRelatedDataProvider<DataProvider.Models.Offer>, IOfferDataProvider
    {

        public OfferDataProvider(CrmContext context, IMapper mapper, ILogger<OfferDataProvider> logger) : base(context, mapper, logger)
        {

        }

        public async Task<Paged<Offer>> GetAllAsync(QuerySet query)
        {
            var source = _context.Offers
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

            // EF Core doesn't support relationships with a derived type so we have to embed the related resource manually
            var offers = QueryListWithManualInclude(source, query);

            var mappedOffers = _mapper.Map<IEnumerable<Offer>>(offers);

            var count = await source.CountAsync();

            return new Paged<Offer>() {
                Items = mappedOffers,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Offer> GetByIdAsync(int id, QuerySet query)
        {
            var source = _context.Offers
                            .Where(c => c.Id == id)
                            .Include(query);

            // EF Core doesn't support relationships with a derived type so we have to embed the related resource manually
            var offer = await QueryWithManualIncludeAsync(source, query);

            if (offer == null)
            {
                _logger.LogError($"No offer found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Offer>(offer);
        }

        public async Task<Paged<Offer>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            var source = _context.Offers
                            .Where(o => o.CustomerId == customerId)
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

            var offers = QueryListWithManualInclude(source, query);

            var mappedOffers = _mapper.Map<IEnumerable<Offer>>(offers);

            var count = await source.CountAsync();

            return new Paged<Offer>() {
                Items = mappedOffers,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };            
        }     
    }
}