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
using System;
using System.Linq.Expressions;

namespace Rollvolet.CRM.DataProviders
{
    public class OfferDataProvider : CaseRelatedDataProvider<DataProvider.Models.Offer>, IOfferDataProvider
    {
        private readonly ISequenceDataProvider _sequenceDataProvider;

        public OfferDataProvider(CrmContext context, IMapper mapper,
                                    ISequenceDataProvider sequenceDataProvider, ILogger<OfferDataProvider> logger) : base(context, mapper, logger)
        {
            _sequenceDataProvider = sequenceDataProvider;
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

        public async Task<Offer> GetByIdAsync(int id, QuerySet query = null)
        {
            var offer = await FindByIdAsync(id, query);

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

        public async Task<Offer> GetByRequestIdAsync(int requestId, QuerySet query = null)
        {
            var offer = await FindWhereAsync(r => r.RequestId == requestId, query);

            if (offer == null)
            {
                _logger.LogError($"No offer found for request-id {requestId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Offer>(offer);
        }

        public async Task<Offer> GetByOrderIdAsync(int orderId, QuerySet query = null)
        {
            var offer = await FindByIdAsync(orderId, query); // offer has the same id as the attached order

            if (offer == null)
            {
                _logger.LogError($"No offer found for order-id {orderId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Offer>(offer);
        }

        public async Task<Offer> GetByOfferlineIdAsync(int offerlineId, QuerySet query = null)
        {
            var offer = await _context.Offerlines.Where(o => o.Id == offerlineId).Select(o => o.Offer).FirstOrDefaultAsync();

            if (offer == null)
            {
                _logger.LogError($"No offer found for offerline-id {offerlineId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Offer>(offer);
        }

        public async Task<Offer> CreateAsync(Offer offer)
        {
            var offerRecord = _mapper.Map<DataProvider.Models.Offer>(offer);

            await UpdateNumberAndSequenceAsync(offerRecord);
            await EmbedCityAsync(offerRecord);
            offerRecord.Currency = "EUR";

            _context.Offers.Add(offerRecord);
            // EF Core requires to create an order record as well because offer and order share the same underlying SQL table
            var orderRecord = _mapper.Map<DataProvider.Models.Order>(offerRecord);
            _context.Orders.Add(orderRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Offer>(offerRecord);
        }

        public async Task<Offer> UpdateAsync(Offer offer)
        {
            var offerRecord = await FindByIdAsync(offer.Id);
            var oldOfferDate = offerRecord.OfferDate;

            _mapper.Map(offer, offerRecord);
            if (oldOfferDate != offerRecord.OfferDate)
            {
                await UpdateNumberAndSequenceAsync(offerRecord);
                _logger.LogDebug($"Offer-date changed. Updating offer number to {offerRecord.Number}");
            }

            await EmbedCityAsync(offerRecord);
            offerRecord.Currency = "EUR";

            _context.Offers.Update(offerRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Offer>(offerRecord);
        }

        public async Task<Offer> UpdateContactAndBuildingAsync(int id, int? relativeContactId, int? relativeBuildingId)
        {
            var offerRecord = await FindByIdAsync(id);
            offerRecord.RelativeContactId = relativeContactId;
            offerRecord.RelativeBuildingId = relativeBuildingId;

            _context.Offers.Update(offerRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Offer>(offerRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var offer = await FindByIdAsync(id);

            if (offer != null)
            {
                _context.Offers.Remove(offer);
                // EF Core requires to delete the order record as well because offer and order share the same underlying SQL table
                var order = await _context.Orders.Where(o => o.Id == id).IgnoreQueryFilters().FirstOrDefaultAsync();
                if (order != null)
                  _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
           }
        }

        private async Task<DataProvider.Models.Offer> FindByIdAsync(int id, QuerySet query = null)
        {
            return await FindWhereAsync(c => c.Id == id, query);
        }

        private async Task<DataProvider.Models.Offer> FindWhereAsync(Expression<Func<DataProvider.Models.Offer, bool>> where, QuerySet query = null)
        {
            var source = _context.Offers.Where(where);

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

        private async Task UpdateNumberAndSequenceAsync(DataProvider.Models.Offer offer)
        {
            var date = offer.OfferDate == null ? DateTime.Now : (DateTime) offer.OfferDate;
            var sequenceNumber = await _sequenceDataProvider.GetNextOfferSequenceNumberAsync(date);
            offer.SequenceNumber = sequenceNumber;
            var offerNumber = $"{(date.Year + 10).ToString().Substring(2, 2)}/{date.ToString("MM")}/{date.ToString("dd")}/{sequenceNumber.ToString("D2")}";
            offer.Number = offerNumber;
        }

        private async Task EmbedCityAsync(DataProvider.Models.Offer offer)
        {
            if (offer.CustomerId != null)
            {
                if (offer.RelativeBuildingId != null)
                {
                    var building = await _context.Buildings
                                            .Where(b => b.Number == offer.RelativeBuildingId && b.CustomerId == offer.CustomerId)
                                            .FirstOrDefaultAsync();
                    offer.EmbeddedCity = building != null ? building.EmbeddedCity : null;
                }
                else
                {
                    var customer = await _context.Customers
                                            .Where(c => c.Number == offer.CustomerId)
                                            .FirstOrDefaultAsync();
                    offer.EmbeddedCity = customer != null ? customer.EmbeddedCity : null;
                }
            }
            else
            {
                offer.EmbeddedCity = null;
            }
        }
    }
}