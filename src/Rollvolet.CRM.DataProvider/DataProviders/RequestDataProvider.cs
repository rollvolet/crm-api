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
    public class RequestDataProvider : CaseRelatedDataProvider<DataProvider.Models.Request>, IRequestDataProvider
    {
        public RequestDataProvider(CrmContext context, IMapper mapper, ILogger<RequestDataProvider> logger) : base(context, mapper, logger)
        {

        }

        public async Task<Paged<Request>> GetAllAsync(QuerySet query)
        {
            var source = _context.Requests
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
            var source = _context.Requests
                            .Where(r => r.CustomerId == customerId)
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

        public async Task<Request> GetByOfferIdAsync(int offerId)
        {
            var request = await _context.Offers.Where(r => r.Id == offerId).Select(r => r.Request).FirstOrDefaultAsync();

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

            await EmbedCity(requestRecord);

            _context.Requests.Add(requestRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Request>(requestRecord);
        }

        public async Task<Request> UpdateAsync(Request request)
        {
            var requestRecord = await FindByIdAsync(request.Id);
            _mapper.Map(request, requestRecord);

            await EmbedCity(requestRecord);

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

        private async Task<DataProvider.Models.Request> FindByIdAsync(int id, QuerySet query = null)
        {
            var source = _context.Requests.Where(c => c.Id == id);

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

        private async Task EmbedCity(DataProvider.Models.Request request)
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