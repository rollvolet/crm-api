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

namespace Rollvolet.CRM.DataProviders
{   
    public class BuildingDataProvider : IBuildingDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;

        public BuildingDataProvider(CrmContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Paged<Building>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            var source = _context.Buildings
                            .Where(c => c.CustomerId == customerId)
                            .Include(query)
                            .Sort(query);

            var buildings = source.Skip(query.Page.Skip).Take(query.Page.Take).AsEnumerable();

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