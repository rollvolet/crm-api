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

namespace Rollvolet.CRM.DataProviders
{   
    public class BuildingDataProvider : IBuildingDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ITelephoneDataProvider _telephoneDataProvider;

        public BuildingDataProvider(CrmContext context, IMapper mapper, ITelephoneDataProvider telephoneDataProvider)
        {
            _context = context;
            _mapper = mapper;
            _telephoneDataProvider = telephoneDataProvider;
        }

        public async Task<Paged<Building>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            var source = _context.Buildings
                            .Where(c => c.CustomerId == customerId)
                            .Include(query)
                            .Sort(query)
                            .Filter(query);

            var buildings = source.ForPage(query).AsEnumerable();

            var count = await source.CountAsync();

            var mappedBuildings = _mapper.Map<IEnumerable<Building>>(buildings);

            return new Paged<Building>() {
                Items = mappedBuildings,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }
    }
}