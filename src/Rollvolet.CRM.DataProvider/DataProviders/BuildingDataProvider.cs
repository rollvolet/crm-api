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
using System;

namespace Rollvolet.CRM.DataProviders
{
    public class BuildingDataProvider : CustomerRecordDataProvider, IBuildingDataProvider
    {
        public BuildingDataProvider(CrmContext context, IMapper mapper, ISequenceDataProvider sequenceDataProvider,
                                    ILogger<BuildingDataProvider> logger) : base(context, mapper, sequenceDataProvider, logger)
        {
        }

        public async Task<Building> GetByIdAsync(int id, QuerySet query = null)
        {
            var building = await FindByIdAsync(id, query);

            if (building == null)
            {
                _logger.LogError($"No building found with data id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Building>(building);
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

        public async Task<Building> CreateAsync(Building building)
        {
            var buildingRecord = _mapper.Map<DataProvider.Models.Building>(building);

            var customerId = building.Customer.Id;
            buildingRecord.Number = await _sequenceDataProvider.GetNextRelativeBuildingNumber(customerId);
            buildingRecord.CustomerId = customerId;
            buildingRecord.Created = DateTime.Now;
            buildingRecord.SearchName = CalculateSearchName(building.Name);

            await HydratePostalCode(building, buildingRecord);

            _context.Buildings.Add(buildingRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Building>(buildingRecord);
        }

        public async Task<Building> UpdateAsync(Building building)
        {
            var buildingRecord = await FindByIdAsync(building.Id);
            _mapper.Map(building, buildingRecord);
            buildingRecord.SearchName = CalculateSearchName(building.Name);

            await HydratePostalCode(building, buildingRecord);

            _context.Buildings.Update(buildingRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Building>(buildingRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var building = await FindByIdAsync(id);

            if (building != null)
            {
                _context.Buildings.Remove(building);
                await _context.SaveChangesAsync();
           }
        }

        private async Task<DataProvider.Models.Building> FindByIdAsync(int id, QuerySet query = null)
        {
            var source = _context.Buildings.Where(c => c.DataId == id);

            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }
    }
}