using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class TelephoneManager : ITelephoneManager
    {
        private readonly ITelephoneDataProvider _telephoneDataProvider;
        private readonly ICountryDataProvider _countryDataProvider;
        private readonly ITelephoneTypeDataProvider _telephoneTypeDataProvider;
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly IContactDataProvider _contactDataProvider;
        private readonly IBuildingDataProvider _buildingDataProvider;
        private readonly ILogger _logger;

        public TelephoneManager(ITelephoneDataProvider telephoneDataProvider, ICountryDataProvider countryDataProvider,
                                ITelephoneTypeDataProvider telephoneTypeDataProvider, ICustomerDataProvider customerDataProvider,
                                IContactDataProvider contactDataProvider, IBuildingDataProvider buildingDataProvider,
                                ILogger<TelephoneManager> logger)
        {
            _telephoneDataProvider = telephoneDataProvider;
            _countryDataProvider = countryDataProvider;
            _telephoneTypeDataProvider = telephoneTypeDataProvider;
            _customerDataProvider = customerDataProvider;
            _contactDataProvider = contactDataProvider;
            _buildingDataProvider = buildingDataProvider;
            _logger = logger;
        }

        public async Task<Paged<Telephone>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            if (query.Sort.Field == null)
                query.Sort.Field = "order";

            return await _telephoneDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }

        public async Task<Paged<Telephone>> GetAllByContactIdAsync(int contactId, QuerySet query)
        {
            if (query.Sort.Field == null)
                query.Sort.Field = "order";

            return await _telephoneDataProvider.GetAllByContactIdAsync(contactId, query);
        }

        public async Task<Paged<Telephone>> GetAllByBuildingIdAsync(int buildingId, QuerySet query)
        {
            if (query.Sort.Field == null)
                query.Sort.Field = "order";

            return await _telephoneDataProvider.GetAllByBuildingIdAsync(buildingId, query);
        }

        public async Task<Telephone> CreateAsync(Telephone telephone)
        {
            // Validations
            if (telephone.Id != null)
                throw new IllegalArgumentException("IllegalAttribute", "Telephone cannot have an id on create.");
            if (telephone.Area == null)
                throw new IllegalArgumentException("IllegalAttribute", "Area is required on telephone creation.");
            if (telephone.Number == null)
                throw new IllegalArgumentException("IllegalAttribute", "Number is required on telephone creation.");
            if (telephone.Country == null)
                throw new IllegalArgumentException("IllegalAttribute", "Country is required on telephone creation.");
            if (telephone.TelephoneType == null)
                throw new IllegalArgumentException("IllegalAttribute", "Telephone-type is required on telephone creation.");
            if (new CustomerEntity[3] { telephone.Customer, telephone.Contact, telephone.Building }.Where(x => x != null).Count() != 1)
                throw new IllegalArgumentException("IllegalAttribute", "Just one of customer, contact or building is required on telephone creation.");

            var number = Regex.Replace(telephone.Number, @"\s+", "");
            if (!Regex.IsMatch(number, @"^[0-9]{6,}$"))
                throw new IllegalArgumentException("IllegalAttribute", "Number must at least contain 6 numbers.");

            var area = Regex.Replace(telephone.Area, @"\s+", "");
            if (!Regex.IsMatch(area, @"^[0-9]{2,4}$"))
                throw new IllegalArgumentException("IllegalAttribute", "Area must contain 2-4 numbers.");

            // Embed existing records
            try {
                var id = int.Parse(telephone.Country.Id);
                telephone.Country = await _countryDataProvider.GetByIdAsync(id);

                id = int.Parse(telephone.TelephoneType.Id);
                telephone.TelephoneType = await _telephoneTypeDataProvider.GetByIdAsync(id);

                if (telephone.Customer != null)
                    telephone.Customer = await _customerDataProvider.GetByNumberAsync(telephone.Customer.Id);
                else if (telephone.Contact != null)
                    telephone.Contact = await _contactDataProvider.GetByIdAsync(telephone.Contact.Id);
                else if (telephone.Building != null)
                    telephone.Building = await _buildingDataProvider.GetByIdAsync(telephone.Building.Id);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }

            return await _telephoneDataProvider.CreateAsync(telephone);
        }

        public async Task DeleteAsync(string composedId)
        {
            await _telephoneDataProvider.DeleteByIdAsync(composedId);
        }
    }
}