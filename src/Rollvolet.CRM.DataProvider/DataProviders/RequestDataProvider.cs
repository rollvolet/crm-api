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
using System.Linq.Expressions;
using System;

namespace Rollvolet.CRM.DataProviders
{
    public class RequestDataProvider : IRequestDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RequestDataProvider(CrmContext context, IMapper mapper, ILogger<RequestDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Paged<Request>> GetAllAsync(QuerySet query)
        {
            var source = _context.Requests
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

            var requests = await source.ForPage(query).ToListAsync();

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

        public async Task<Request> GetByOfferIdAsync(int offerId, QuerySet query = null)
        {
            var requestId = await _context.Offers.Where(r => r.Id == offerId).Select(r => r.RequestId).FirstOrDefaultAsync();

            if (requestId == null)
            {
                _logger.LogError($"No request found for offer id {offerId}");
                throw new EntityNotFoundException();
            }

            var request = await FindByIdAsync((int) requestId, query);
            return _mapper.Map<Request>(request);
        }

        public async Task<Request> GetByOrderIdAsync(int orderId, QuerySet query = null)
        {
            return await GetByOfferIdAsync(orderId, query);
        }

        public async Task<Request> GetByInterventionIdAsync(int interventionId)
        {
            var request = await FindWhereAsync(r => r.OriginId == interventionId);

            if (request == null)
            {
                _logger.LogError($"No request found for intervention id {interventionId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Request>(request);
        }

        public async Task<Request> CreateAsync(Request request)
        {
            var requestRecord = _mapper.Map<DataProvider.Models.Request>(request);
            await EmbedCityAsync(requestRecord);

            _context.Requests.Add(requestRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Request>(requestRecord);
        }

        public async Task<Request> UpdateAsync(Request request)
        {
            var requestRecord = await FindByIdAsync(request.Id);
            _mapper.Map(request, requestRecord);

            await EmbedCityAsync(requestRecord);

            _context.Requests.Update(requestRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Request>(requestRecord);
        }

        public async Task<Request> UpdateContactAndBuildingAsync(int id, int? relativeContactId, int? relativeBuildingId)
        {
            var requestRecord = await FindByIdAsync(id);
            requestRecord.RelativeContactId = relativeContactId;
            requestRecord.RelativeBuildingId = relativeBuildingId;

            _context.Requests.Update(requestRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Request>(requestRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var request = await FindByIdAsync(id);

            if (request != null)
            {
                _context.Requests.Remove(request);
                await _context.SaveChangesAsync();
           }
        }

        private async Task<Paged<Request>> GetAllWhereAsync(Expression<Func<DataProvider.Models.Request, bool>> where, QuerySet query)
        {
            var source = _context.Requests
                            .Where(where)
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

            var requests = await source.ForPage(query).ToListAsync();

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
            var source = (IQueryable<DataProvider.Models.Request>) _context.Requests.Where(where);
            
            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
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
    }
}