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

        public async Task<Building> GetByTelephoneIdAsync(string telephoneId)
        {
            return await _buildingDataProvider.GetByTelephoneIdAsync(telephoneId);
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

            await EmbedRelations(building);

            return await _buildingDataProvider.CreateAsync(building);
        }

        public async Task<Building> UpdateAsync(Building building)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer", "country", "language", "honorific-prefix" };
            var existingBuilding = await _buildingDataProvider.GetByIdAsync(building.Id, query);

            if (building.Number != existingBuilding.Number)
                throw new IllegalArgumentException("IllegalAttribute", "Building number cannot be updated.");
            if ((building.PostalCode != null && building.City == null) || (building.PostalCode == null && building.City != null))
                throw new IllegalArgumentException("IllegalAttribute", "Building's postal-code and city must be both filled in or not filled.");
            if (building.Country == null)
                throw new IllegalArgumentException("IllegalAttribute", "Country is required.");
            if (building.Language == null)
                throw new IllegalArgumentException("IllegalAttribute", "Language is required.");
            if (building.Customer != null && building.Customer.Id != existingBuilding.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Customer cannot be updated.");
            if (building.Telephones != null)
            {
                var message = "Telephones cannot be change during building update.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }

            await EmbedRelations(building, existingBuilding);

            return await _buildingDataProvider.UpdateAsync(building);
        }

        public async Task DeleteAsync(int id)
        {
            await _buildingDataProvider.DeleteByIdAsync(id);
        }

        // Embed relations in contact resource: use old embedded value if there is one and the relation isn't updated or null
        private async Task EmbedRelations(Building building, Building oldBuilding = null)
        {
            try {
                if (building.Country != null)
                {
                    if (oldBuilding != null && oldBuilding.Country != null && oldBuilding.Country.Id == building.Country.Id)
                        building.Country = oldBuilding.Country;
                    else
                        building.Country = await _countryDataProvider.GetByIdAsync(int.Parse(building.Country.Id));
                }

                if (building.Language != null)
                {
                    if (oldBuilding != null && oldBuilding.Language != null && oldBuilding.Language.Id == building.Language.Id)
                        building.Language = oldBuilding.Language;
                    else
                        building.Language = await _langugageDataProvider.GetByIdAsync(int.Parse(building.Language.Id));
                }

                if (building.HonorificPrefix != null)
                {
                    if (oldBuilding != null && oldBuilding.HonorificPrefix != null && oldBuilding.HonorificPrefix.Id == building.HonorificPrefix.Id)
                        building.HonorificPrefix = oldBuilding.HonorificPrefix;
                    else
                        building.HonorificPrefix = await _honorificPrefixDataProvider.GetByIdAsync(building.HonorificPrefix.Id);
                }

                // Customer cannot be updated. Take customer of oldBuilding on update.
                if (oldBuilding != null)
                    building.Customer = oldBuilding.Customer;
                else
                    building.Customer = await _customerDataProvider.GetByNumberAsync(building.Customer.Id);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }

        }

    }
}
