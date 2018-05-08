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

            await EmbedRelations(contact);

            return await _contactDataProvider.CreateAsync(contact);
        }

        public async Task<Contact> UpdateAsync(Contact contact)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer", "country", "language", "honorific-prefix" };
            var existingContact = await _contactDataProvider.GetByIdAsync(contact.Id, query);

            if (contact.Number != existingContact.Number)
                throw new IllegalArgumentException("IllegalAttribute", "Contact number cannot be updated.");
            if ((contact.PostalCode != null && contact.City == null) || (contact.PostalCode == null && contact.City != null))
                throw new IllegalArgumentException("IllegalAttribute", "Contact's postal-code and city must be both filled in or not filled.");

            // TODO handle telephones

            await EmbedRelations(contact, existingContact);

            if (contact.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required.");
            if (contact.Country == null)
                throw new IllegalArgumentException("IllegalAttribute", "Country is required.");
            if (contact.Language == null)
                throw new IllegalArgumentException("IllegalAttribute", "Language is required.");

            return await _contactDataProvider.UpdateAsync(contact);
        }

        public async Task DeleteAsync(int id)
        {
            await _contactDataProvider.DeleteByIdAsync(id);
        }

        // Embed relations in contact resource: use old embedded value if there is one and the relation isn't updated or null
        private async Task EmbedRelations(Contact contact, Contact oldContact = null)
        {
            try {
                if (contact.Country != null && (oldContact == null || oldContact.Country == null || contact.Country.Id != oldContact.Country.Id) )
                {
                    var id = int.Parse(contact.Country.Id);
                    contact.Country = await _countryDataProvider.GetByIdAsync(id);
                }
                else
                {
                    contact.Country = oldContact != null ? oldContact.Country : null;
                }

                if (contact.Language != null && (oldContact == null || oldContact.Language == null || contact.Language.Id != oldContact.Language.Id) )
                {
                    var id = int.Parse(contact.Language.Id);
                    contact.Language = await _langugageDataProvider.GetByIdAsync(id);
                }
                else
                {
                    contact.Language = oldContact != null ? oldContact.Language : null;
                }

                if (contact.Customer != null && (oldContact == null || oldContact.Customer == null || contact.Customer.Id != oldContact.Customer.Id) )
                {
                    var id = contact.Customer.Id;
                    contact.Customer = await _customerDataProvider.GetByNumberAsync(id);
                }
                else
                {
                    contact.Customer = oldContact != null ? oldContact.Customer : null;
                }

                if (contact.HonorificPrefix != null &&
                        (oldContact == null || oldContact.HonorificPrefix == null || contact.HonorificPrefix.Id != oldContact.HonorificPrefix.Id) )
                {
                    var composedId = contact.HonorificPrefix.Id;
                    contact.HonorificPrefix = await _honorificPrefixDataProvider.GetByIdAsync(composedId);
                }
                else
                {
                    contact.HonorificPrefix = oldContact != null ? oldContact.HonorificPrefix : null;
                }
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }

        }
    }
}