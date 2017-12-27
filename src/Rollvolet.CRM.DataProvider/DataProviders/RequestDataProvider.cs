using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Narato.ResponseMiddleware.Models.Exceptions;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.DataProvider.Extensions;
using Microsoft.Extensions.Logging;

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
                            .Filter(query);

            var requests = source.Skip(query.Page.Skip).Take(query.Page.Take).AsEnumerable();

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

            var request = await source.FirstOrDefaultAsync();

            if (request == null)
            {
                // TODO implement and handle exceptions according to jsonapi
                _logger.LogError($"No request found with id {id}");
                throw new EntityNotFoundException("ENF", $"Request with id {id} not found");
            }

            return _mapper.Map<Request>(request);
        }
    }
}