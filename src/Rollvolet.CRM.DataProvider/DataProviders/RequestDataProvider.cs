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

            var requests = await source.Skip(query.Page.Skip).Take(query.Page.Take).ToListAsync();

            // EF Core doesn't support IncludeWhere so we have to embed the related resource manually
            if (query.Include.Fields.Contains("building"))
                await EmbedBuilding(requests);

            if (query.Include.Fields.Contains("contact"))
                await EmbedContact(requests);

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

            // EF Core doesn't support IncludeWhere so we have to embed the related resource manually
            if (query.Include.Fields.Contains("building"))
                await EmbedBuilding(request);

            if (query.Include.Fields.Contains("contact"))
                await EmbedContact(request);

            return _mapper.Map<Request>(request);
        }

        private async Task EmbedBuilding(DataProvider.Models.Request request)
        {
            await EmbedBuilding(new List<DataProvider.Models.Request> { request });
        }

        private async Task EmbedBuilding(IList<DataProvider.Models.Request> requests)
        {
            var requestWithBuilding = requests.Where(x => x.RelativeBuildingId != null);

            if (requestWithBuilding.Count() > 0)
            {
                var relatedBuildings = requestWithBuilding.Select(x => new { Number = x.RelativeBuildingId, CustomerId = x.CustomerId });

                var whereClause = PredicateBuilder.New<DataProvider.Models.Building>(false);

                foreach(var building in relatedBuildings)
                {
                    whereClause.Or(x => x.Number == building.Number && x.CustomerId == building.CustomerId);
                }

                var buildings = await _context.Buildings.Where(whereClause).ToListAsync();

                foreach(var r in requestWithBuilding)
                {
                    r.Building = buildings.Where(x => x.Number == r.RelativeBuildingId && x.CustomerId == r.CustomerId).FirstOrDefault();
                }
            }
        }

        private async Task EmbedContact(DataProvider.Models.Request request)
        {
            await EmbedContact(new List<DataProvider.Models.Request> { request });
        }

        private async Task EmbedContact(IList<DataProvider.Models.Request> requests)
        {
            var requestsWithContacts = requests.Where(x => x.RelativeContactId != null);

            if (requestsWithContacts.Count() > 0)
            {
                var relatedContacts = requestsWithContacts.Select(x => new { Number = x.RelativeContactId, CustomerId = x.CustomerId });

                var whereClause = PredicateBuilder.New<DataProvider.Models.Contact>(false);

                foreach(var contact in relatedContacts)
                {
                    whereClause.Or(x => x.Number == contact.Number && x.CustomerId == contact.CustomerId);
                }

                var contacts = await _context.Contacts.Where(whereClause).ToListAsync();

                foreach(var r in requestsWithContacts)
                {
                    r.Contact = contacts.Where(x => x.Number == r.RelativeBuildingId && x.CustomerId == r.CustomerId).FirstOrDefault();
                }
            }
        }        
    }
}