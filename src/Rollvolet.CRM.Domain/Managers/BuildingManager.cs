using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class BuildingManager : IBuildingManager
    {
        private readonly IBuildingDataProvider _buildingDataProvider;
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly ICountryDataProvider _countryDataProvider;
        private readonly IHonorificPrefixDataProvider _honorificPrefixDataProvider;
        private readonly ILanguageDataProvider _langugageDataProvider;
        private readonly ILogger _logger;

        public BuildingManager(IBuildingDataProvider buildingDataProvider, ICustomerDataProvider customerDataProvider, 
                                ICountryDataProvider countryDataProvider, IHonorificPrefixDataProvider honorificPrefixDataProvider, 
                                ILanguageDataProvider languageDataProvider, ILogger<BuildingManager> logger)
        {
            _buildingDataProvider = buildingDataProvider;
            _customerDataProvider = customerDataProvider;
            _countryDataProvider = countryDataProvider;
            _honorificPrefixDataProvider = honorificPrefixDataProvider;
            _langugageDataProvider = languageDataProvider;
            _logger = logger;
        }
        
        public async Task<Paged<Building>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            if (query.Sort.Field == null)
                query.Sort.Field = "name";

            return await _buildingDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }

        public async Task<Building> GetByIdAsync(int id, QuerySet query)
        {
            return await _buildingDataProvider.GetByIdAsync(id, query);
        }

        public async Task<Building> CreateAsync(Building building)
        {
            // Validations
            if (building.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Building cannot have an id on create.");
            if (building.Number != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Building cannot have a number on create.");
            if ((building.PostalCode != null && building.City == null) || (building.PostalCode == null && building.City != null))
                throw new IllegalArgumentException("IllegalAttribute", "Building's postal-code and city must be both filled in or not filled.");
            if (building.Telephones != null)
            {
                var message = "Telephones cannot be added to a building on creation.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }
            if (building.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required on building creation.");
            if (building.Country == null)
                throw new IllegalArgumentException("IllegalAttribute", "Country is required on building creation.");
            if (building.Language == null)
                throw new IllegalArgumentException("IllegalAttribute", "Language is required on building creation.");

            // Embed existing records
            try {
                var id = int.Parse(building.Country.Id);
                building.Country = await _countryDataProvider.GetByIdAsync(id);

                id = int.Parse(building.Language.Id);
                building.Language = await _langugageDataProvider.GetByIdAsync(id);

                id = building.Customer.Id;
                building.Customer = await _customerDataProvider.GetByNumberAsync(id);

                if (building.HonorificPrefix != null)
                {
                    var composedId = building.HonorificPrefix.Id;
                    building.HonorificPrefix = await _honorificPrefixDataProvider.GetByIdAsync(composedId);
                }
            }
            catch (EntityNotFoundException) 
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }

            return await _buildingDataProvider.CreateAsync(building);
        }

        public async Task DeleteAsync(int id)
        {
            await _buildingDataProvider.DeleteByIdAsync(id);
        }
    }
}