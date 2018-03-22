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

        public async Task<Request> GetByIdAsync(int id, QuerySet query)
        {
            var source = _context.Requests
                            .Where(c => c.Id == id)
                            .Include(query);

            // EF Core doesn't support relationships with a derived type so we have to embed the related resource manually
            var request = await QueryWithManualIncludeAsync(source, query);

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
    }
}