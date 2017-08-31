using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class BuildingManager : IBuildingManager
    {
        private readonly IBuildingDataProvider _buildingDataProvider;

        public BuildingManager(IBuildingDataProvider buildingDataProvider)
        {
            _buildingDataProvider = buildingDataProvider;
        }
        
        public async Task<Paged<Building>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            return await _buildingDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }
    }
}