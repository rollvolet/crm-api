using System;
using System.Threading;
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
        private readonly IRequestDataProvider _requestDataProvider;
        private readonly IInvoiceDataProvider _invoiceDataProvider;
        private readonly ILogger _logger;

        public ContactManager(IContactDataProvider contactDataProvider, ICustomerDataProvider customerDataProvider,
                                ICountryDataProvider countryDataProvider, IHonorificPrefixDataProvider honorificPrefixDataProvider,
                                IRequestDataProvider requestDataProvider, IInvoiceDataProvider invoiceDataProvider,
                                ILanguageDataProvider languageDataProvider, ILogger<ContactManager> logger)
        {
            _contactDataProvider = contactDataProvider;
            _customerDataProvider = customerDataProvider;
            _countryDataProvider = countryDataProvider;
            _honorificPrefixDataProvider = honorificPrefixDataProvider;
            _langugageDataProvider = languageDataProvider;
            _requestDataProvider = requestDataProvider;
            _invoiceDataProvider = invoiceDataProvider;
            _logger = logger;
        }

        public async Task<Paged<Contact>> GetAllAsync(QuerySet query)
        {
            if (query.Filter.Fields.ContainsKey("customer.number"))
            {
                var filterValue = query.Filter.Fields["customer.number"];
                int customerId;
                if (Int32.TryParse(filterValue, out customerId)) {
                    return await GetAllByCustomerIdAsync(customerId, query);
                }
                else
                {
                    throw new IllegalArgumentException("IllegalFilter", "Customer number filter must be a integer.");
                }
            }
            else
            {
                throw new IllegalArgumentException("IllegalFilter", "Customer number filter is required to get all contacts.");
            }
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

        public async Task<Contact> GetByTelephoneIdAsync(string telephoneId)
        {
            return await _contactDataProvider.GetByTelephoneIdAsync(telephoneId);
        }

        public async Task<Contact> GetByRequestIdAsync(int requestId)
        {
            return await _contactDataProvider.GetByRequestIdAsync(requestId);
        }

        public async Task<Contact> GetByOfferIdAsync(int offerId)
        {
            return await _contactDataProvider.GetByOfferIdAsync(offerId);
        }

        public async Task<Contact> GetByOrderIdAsync(int orderId)
        {
            return await _contactDataProvider.GetByOrderIdAsync(orderId);
        }

        public async Task<Contact> GetByInvoiceIdAsync(int invoiceId)
        {
            return await _contactDataProvider.GetByInvoiceIdAsync(invoiceId);
        }

        public async Task<Contact> GetByDepositInvoiceIdAsync(int depositInvoiceId)
        {
            return await _contactDataProvider.GetByDepositInvoiceIdAsync(depositInvoiceId);
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

            if (contact.City != null)
                contact.City = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(contact.City.ToLower());

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
            if (contact.Country == null)
                throw new IllegalArgumentException("IllegalAttribute", "Country is required.");
            if (contact.Language == null)
                throw new IllegalArgumentException("IllegalAttribute", "Language is required.");
            if (contact.Customer != null && contact.Customer.Id != existingContact.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Customer cannot be updated.");
            if (contact.Telephones != null)
            {
                var message = "Telephones cannot be changed during contact update.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }

            await EmbedRelations(contact, existingContact);

            if (contact.City != null)
                contact.City = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(contact.City.ToLower());

            return await _contactDataProvider.UpdateAsync(contact);
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var includeCustomer = new QuerySet();
                includeCustomer.Include.Fields = new string[] { "customer" };
                var contact = await _contactDataProvider.GetByIdAsync(id, includeCustomer);

                var query = new QuerySet();
                query.Page.Size = 1;

                var requests = await _requestDataProvider.GetAllByRelativeContactIdAsync(contact.Customer.Id, contact.Number, query);
                if (requests.Count > 0)
                {
                    _logger.LogError($"Contact {id} cannot be deleted because requests are still attached to it.");
                    throw new InvalidOperationException($"Contact {id} cannot be deleted because requests are still attached to it.");
                }

                var invoices = await _invoiceDataProvider.GetAllByRelativeContactIdAsync(contact.Customer.Id, contact.Number, query);
                if (invoices.Count > 0)
                {
                    _logger.LogError($"Contact {id} cannot be deleted because invoices are still attached to it.");
                    throw new InvalidOperationException($"Contact {id} cannot be deleted because invoices are still attached to it.");
                }

                await _contactDataProvider.DeleteByIdAsync(id);
            }
            catch (EntityNotFoundException)
            {
                // Contact not found. Nothing to delete.
            }
        }

        // Embed relations in contact resource: use old embedded value if there is one and the relation isn't updated or null
        private async Task EmbedRelations(Contact contact, Contact oldContact = null)
        {
            try {
                if (contact.Country != null)
                {
                    if (oldContact != null && oldContact.Country != null && oldContact.Country.Id == contact.Country.Id)
                        contact.Country = oldContact.Country;
                    else
                        contact.Country = await _countryDataProvider.GetByIdAsync(int.Parse(contact.Country.Id));
                }

                if (contact.Language != null)
                {
                    if (oldContact != null && oldContact.Language != null && oldContact.Language.Id == contact.Language.Id)
                        contact.Language = oldContact.Language;
                    else
                        contact.Language = await _langugageDataProvider.GetByIdAsync(int.Parse(contact.Language.Id));
                }

                if (contact.HonorificPrefix != null)
                {
                    if (oldContact != null && oldContact.HonorificPrefix != null && oldContact.HonorificPrefix.Id == contact.HonorificPrefix.Id)
                        contact.HonorificPrefix = oldContact.HonorificPrefix;
                    else
                        contact.HonorificPrefix = await _honorificPrefixDataProvider.GetByIdAsync(contact.HonorificPrefix.Id);
                }

                // Customer cannot be updated. Take customer of oldContact on update.
                if (oldContact != null)
                    contact.Customer = oldContact.Customer;
                else
                    contact.Customer = await _customerDataProvider.GetByNumberAsync(contact.Customer.Id);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }
    }
}
