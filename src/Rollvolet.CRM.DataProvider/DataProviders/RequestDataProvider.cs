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
using System.Linq.Expressions;
using System;

namespace Rollvolet.CRM.DataProviders
{
    public class RequestDataProvider : CaseRelatedDataProvider<DataProvider.Models.Request>, IRequestDataProvider
    {
        public RequestDataProvider(CrmContext context, IMapper mapper, ILogger<RequestDataProvider> logger) : base(context, mapper, logger)
        {

        }

        public async Task<Paged<Request>> GetAllAsync(QuerySet query)
        {
            var source = _context.Requests
                            .Include(r => r.Visit) // required inclusion to embed properties in domain object
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

            // EF Core doesn't support relationships with a derived type so we have to embed the related resource manually
            var requests = QueryListWithManualInclude(source, query);

            var mappedRequests = _mapper.Map<IEnumerable<Request>>(requests);

            var count = await source.CountAsync();

            return new Paged<Request>() {
                Items = mappedRequests,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Request> GetByIdAsync(int id, QuerySet query = null)
        {
            var request = await FindByIdAsync(id, query);

            if (request == null)
            {
                _logger.LogError($"No request found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Request>(request);
        }

        public async Task<Paged<Request>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            return await GetAllWhereAsync(r => r.CustomerId == customerId, query);
        }

        public async Task<Paged<Request>> GetAllByRelativeContactIdAsync(int customerId, int relativeContactId, QuerySet query)
        {
            return await GetAllWhereAsync(r => r.CustomerId == customerId && r.RelativeContactId == relativeContactId, query);
        }

        public async Task<Paged<Request>> GetAllByRelativeBuildingIdAsync(int customerId, int relativeBuildingId, QuerySet query)
        {
            return await GetAllWhereAsync(r => r.CustomerId == customerId && r.RelativeBuildingId == relativeBuildingId, query);
        }

        public async Task<Request> GetByOfferIdAsync(int offerId)
        {
            var request = await _context.Offers.Where(r => r.Id == offerId)
                                    .Select(r => r.Request).Include(r => r.Visit).FirstOrDefaultAsync(); // required inclusion to embed properties in domain object

            if (request == null)
            {
                _logger.LogError($"No request found for offer id {offerId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Request>(request);
        }

        public async Task<Request> GetByOrderIdAsync(int orderId)
        {
            return await GetByOfferIdAsync(orderId);
        }


        public async Task<Request> CreateAsync(Request request)
        {
            var requestRecord = _mapper.Map<DataProvider.Models.Request>(request);
            await EmbedCityAsync(requestRecord);

            _context.Requests.Add(requestRecord);
            await _context.SaveChangesAsync();

            _logger.LogDebug($"Automatically create visit record for new request with id {requestRecord.Id}.");
            var visit = await CreateVisitForRequestAsync(requestRecord, request);

            request = _mapper.Map<Request>(requestRecord);
            return _mapper.Map(visit, request);
        }

        public async Task<Request> UpdateAsync(Request request)
        {
            var requestRecord = await FindByIdAsync(request.Id);
            _mapper.Map(request, requestRecord);

            await EmbedCityAsync(requestRecord);

            _context.Requests.Update(requestRecord);
            await _context.SaveChangesAsync();

            _logger.LogDebug($"Automatically update visit record for existing request with id {requestRecord.Id}.");
            var visit = await UpdateVisitForRequestAsync(requestRecord, request);

            request = _mapper.Map<Request>(requestRecord);
            return _mapper.Map(visit, request);
        }

        public async Task<Request> UpdateContactAndBuildingAsync(int id, int? relativeContactId, int? relativeBuildingId)
        {
            var requestRecord = await FindByIdAsync(id);
            requestRecord.RelativeContactId = relativeContactId;
            requestRecord.RelativeBuildingId = relativeBuildingId;

            _context.Requests.Update(requestRecord);
            await _context.SaveChangesAsync();

            _logger.LogDebug($"Automatically update visit record for existing request with id {requestRecord.Id}.");
            await UpdateVisitForRequestAsync(requestRecord);

            return _mapper.Map<Request>(requestRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var request = await FindByIdAsync(id);

            if (request != null)
            {
                var visit = await _context.Requests.Where(c => c.Id == id).Select(c => c.Visit).FirstOrDefaultAsync();

                if (visit != null)
                    _context.Visits.Remove(visit);

                _context.Requests.Remove(request);
                await _context.SaveChangesAsync();
           }
        }

        private async Task<Paged<Request>> GetAllWhereAsync(Expression<Func<DataProvider.Models.Request, bool>> where, QuerySet query)
        {
            var source = _context.Requests
                            .Where(where)
                            .Include(r => r.Visit) // required inclusion to embed properties in domain object
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

            var requests = QueryListWithManualInclude(source, query);

            var mappedRequests = _mapper.Map<IEnumerable<Request>>(requests);

            var count = await source.CountAsync();

            return new Paged<Request>() {
                Items = mappedRequests,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        private async Task<DataProvider.Models.Request> FindByIdAsync(int id, QuerySet query = null)
        {
            return await FindWhereAsync(c => c.Id == id, query);
        }

        private async Task<DataProvider.Models.Request> FindWhereAsync(Expression<Func<DataProvider.Models.Request, bool>> where, QuerySet query = null)
        {
            var source = (IQueryable<DataProvider.Models.Request>) _context.Requests.Where(where).Include(r => r.Visit); // required inclusion to embed properties in domain object

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

        private async Task EmbedCityAsync(DataProvider.Models.Request request)
        {
            if (request.CustomerId != null)
            {
                if (request.RelativeBuildingId != null)
                {
                    var building = await _context.Buildings.Where(b => b.Number == request.RelativeBuildingId && b.CustomerId == request.CustomerId)
                                                .FirstOrDefaultAsync();
                    request.EmbeddedCity = building != null ? building.EmbeddedCity : null;
                }
                else
                {
                    var customer = await _context.Customers.Where(c => c.Number == request.CustomerId).FirstOrDefaultAsync();
                    request.EmbeddedCity = customer != null ? customer.EmbeddedCity : null;
                }
            }
            else
            {
                request.EmbeddedCity = null;
            }
        }

        private async Task<DataProvider.Models.Visit> CreateVisitForRequestAsync(DataProvider.Models.Request requestRecord, Request request = null)
        {
            var visitRecord = new DataProvider.Models.Visit {
                RequestId = requestRecord.Id,
                CustomerId = requestRecord.CustomerId,
                RelativeContactId = requestRecord.RelativeContactId,
                RelativeBuildingId = requestRecord.RelativeBuildingId,
                EmbeddedCity = requestRecord.EmbeddedCity,
                Comment =  request == null ? null : request.Comment,
                Visitor = request == null ? null : request.Visitor,
                OfferExpected = request == null ? false : request.OfferExpected
            };

            _context.Visits.Add(visitRecord);
            await _context.SaveChangesAsync();

            return visitRecord;
        }

        private async Task<DataProvider.Models.Visit> UpdateVisitForRequestAsync(DataProvider.Models.Request requestRecord, Request request = null)
        {
            var visit = await _context.Requests.Where(c => c.Id == requestRecord.Id).Select(c => c.Visit).FirstOrDefaultAsync();

            if (visit == null)
            {
                _logger.LogWarning($"No visit found for request with id {requestRecord.Id}. Creating one now.");
                return await CreateVisitForRequestAsync(requestRecord, request);
            }
            else
            {
                visit.RequestId = requestRecord.Id;
                visit.CustomerId = requestRecord.CustomerId;
                visit.RelativeContactId = requestRecord.RelativeContactId;
                visit.RelativeBuildingId = requestRecord.RelativeBuildingId;
                visit.EmbeddedCity = requestRecord.EmbeddedCity;

                if (request != null)
                {
                    visit.Visitor = request.Visitor;
                    visit.OfferExpected = request.OfferExpected;
                    visit.Comment = request.Comment;
                }

               _context.Visits.Update(visit);
                await _context.SaveChangesAsync();
                return visit;
            }
        }
    }
}