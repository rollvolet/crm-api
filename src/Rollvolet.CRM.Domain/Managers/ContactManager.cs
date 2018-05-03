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
    public class ContactManager : IContactManager
    {
        private readonly IContactDataProvider _contactDataProvider;
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly ICountryDataProvider _countryDataProvider;
        private readonly IHonorificPrefixDataProvider _honorificPrefixDataProvider;
        private readonly ILanguageDataProvider _langugageDataProvider;
        private readonly ILogger _logger;

        public ContactManager(IContactDataProvider contactDataProvider, ICustomerDataProvider customerDataProvider, 
                                ICountryDataProvider countryDataProvider, IHonorificPrefixDataProvider honorificPrefixDataProvider, 
                                ILanguageDataProvider languageDataProvider, ILogger<ContactManager> logger)
        {
            _contactDataProvider = contactDataProvider;
            _customerDataProvider = customerDataProvider;
            _countryDataProvider = countryDataProvider;
            _honorificPrefixDataProvider = honorificPrefixDataProvider;
            _langugageDataProvider = languageDataProvider;
            _logger = logger;
        }
        
        public async Task<Paged<Contact>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            if (query.Sort.Field == null)
                query.Sort.Field = "name";
                
            return await _contactDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }

        public async Task<Contact> GetByIdAsync(int id, QuerySet query)
        {
            return await _contactDataProvider.GetByIdAsync(id, query);
        }

        public async Task<Contact> CreateAsync(Contact contact)
        {
            // Validations
            if (contact.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Contact cannot have an id on create.");
            if (contact.Number != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Contact cannot have a number on create.");
            if ((contact.PostalCode != null && contact.City == null) || (contact.PostalCode == null && contact.City != null))
                throw new IllegalArgumentException("IllegalAttribute", "Contact's postal-code and city must be both filled in or not filled.");
            if (contact.Telephones != null)
            {
                var message = "Telephones cannot be added to a contact on creation.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }
            if (contact.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required on contact creation.");
            if (contact.Country == null)
                throw new IllegalArgumentException("IllegalAttribute", "Country is required on contact creation.");
            if (contact.Language == null)
                throw new IllegalArgumentException("IllegalAttribute", "Language is required on contact creation.");

            // Embed existing records
            try {
                var id = int.Parse(contact.Country.Id);
                contact.Country = await _countryDataProvider.GetByIdAsync(id);

                id = int.Parse(contact.Language.Id);
                contact.Language = await _langugageDataProvider.GetByIdAsync(id);

                id = contact.Customer.Id;
                contact.Customer = await _customerDataProvider.GetByNumberAsync(id);

                if (contact.HonorificPrefix != null)
                {
                    var composedId = contact.HonorificPrefix.Id;
                    contact.HonorificPrefix = await _honorificPrefixDataProvider.GetByIdAsync(composedId);
                }
            }
            catch (EntityNotFoundException) 
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }

            return await _contactDataProvider.CreateAsync(contact);
        }

        public async Task DeleteAsync(int id)
        {
            await _contactDataProvider.DeleteByIdAsync(id);
        }
    }
}