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
using LinqKit;

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
                // TODO implement and handle exceptions according to jsonapi
                _logger.LogError($"No request found with id {id}");
                throw new EntityNotFoundException("ENF", $"Request with id {id} not found");
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

        private IEnumerable<DataProvider.Models.Request> QueryListWithManualInclude(IQueryable<DataProvider.Models.Request> source, QuerySet query)
        {
            if (query.Include.Fields.Contains("building")  || query.Include.Fields.Contains("contact"))
            {
                var joinedSource = JoinBuildingAndContact(source);
                var triplets = joinedSource.ForPage(query).AsEnumerable();
                return EmbedBuildingAndContact(triplets);
            }
            else
            {
                return source.ForPage(query).AsEnumerable();
            }
        }
        private async Task<DataProvider.Models.Request> QueryWithManualIncludeAsync(IQueryable<DataProvider.Models.Request> source, QuerySet query)
        {
            if (query.Include.Fields.Contains("building")  || query.Include.Fields.Contains("contact"))
            {
                var joinedSource = JoinBuildingAndContact(source);
                var triplet = await joinedSource.FirstOrDefaultAsync();
                return EmbedBuildingAndContact(triplet);
            }
            else
            {
                return await source.FirstOrDefaultAsync();
            }
        }

        private IQueryable<DataProvider.Models.Request.Triplet> JoinBuildingAndContact(IQueryable<DataProvider.Models.Request> source)
        {
            return source.GroupJoin(
                    _context.Buildings,
                    r => new { CustomerId = r.CustomerId, Number = r.RelativeBuildingId },
                    b => new { CustomerId = (int?) b.CustomerId, Number = (int?) b.Number },
                    (r, b) => new { Request = r, Buildings = b }
                ).SelectMany(
                    t => t.Buildings.DefaultIfEmpty(),
                    (t, b) => new { Request = t.Request, Building = b }
                ).GroupJoin(
                    _context.Contacts,
                    t => new { CustomerId = t.Request.CustomerId, Number = t.Request.RelativeContactId },
                    c => new { CustomerId = (int?) c.CustomerId, Number = (int?) c.Number },
                    (t, c) => new { Request = t.Request, Building = t.Building, Contacts = c }
                ).SelectMany(
                    u => u.Contacts.DefaultIfEmpty(),
                    (u, c) => new DataProvider.Models.Request.Triplet { Request = u.Request, Building = u.Building, Contact = c}
                );
        }

        private DataProvider.Models.Request EmbedBuildingAndContact(DataProvider.Models.Request.Triplet triplet)
        {
            var request = triplet.Request;

            if (request != null) {
                request.Building = triplet.Building;
                request.Contact = triplet.Contact;
            }

            return request;
        }

        private IEnumerable<DataProvider.Models.Request> EmbedBuildingAndContact(IEnumerable<DataProvider.Models.Request.Triplet> triplets)
        {
            var requests = new List<DataProvider.Models.Request>();

            foreach(var triplet in triplets)
            {
                var request = triplet.Request;
                request.Building = triplet.Building;
                request.Contact = triplet.Contact;
                requests.Add(request);
            }

            return requests;
        }       
    }
}