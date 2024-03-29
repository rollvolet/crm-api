using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class CustomerManager : ICustomerManager
    {
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly ICountryDataProvider _countryDataProvider;
        private readonly IHonorificPrefixDataProvider _honorificPrefixDataProvider;
        private readonly ILanguageDataProvider _langugageDataProvider;
        private readonly ITagDataProvider _tagDataProvider;
        private readonly IContactDataProvider _contactDataProvider;
        private readonly IBuildingDataProvider _buildingDataProvider;
        private readonly IRequestDataProvider _requestDataProvider;
        private readonly IInvoiceDataProvider _invoiceDataProvider;
        private readonly ILogger _logger;

        public CustomerManager(ICustomerDataProvider customerDataProvider, ICountryDataProvider countryDataProvider,
                                IHonorificPrefixDataProvider honorificPrefixDataProvider, ILanguageDataProvider languageDataProvider,
                                IContactDataProvider contactDataProvider, IBuildingDataProvider buildingDataProvider,
                                IRequestDataProvider requestDataProvider, IInvoiceDataProvider invoiceDataProvider,
                                ITagDataProvider tagDataProvider, ILogger<CustomerManager> logger)
        {
            _customerDataProvider = customerDataProvider;
            _countryDataProvider = countryDataProvider;
            _honorificPrefixDataProvider = honorificPrefixDataProvider;
            _langugageDataProvider = languageDataProvider;
            _tagDataProvider = tagDataProvider;
            _contactDataProvider = contactDataProvider;
            _buildingDataProvider = buildingDataProvider;
            _requestDataProvider = requestDataProvider;
            _invoiceDataProvider = invoiceDataProvider;
            _logger = logger;
        }

        public async Task<Paged<Customer>> GetAllAsync(QuerySet query)
        {
            if (query.Sort.Field == null)
                query.Sort.Field = "name";

            return await _customerDataProvider.GetAllAsync(query);
        }

        public async Task<Customer> GetByIdAsync(int id, QuerySet query)
        {
            return await _customerDataProvider.GetByNumberAsync(id, query);
        }

        public async Task<Customer> GetByRequestIdAsync(int requestId)
        {
            return await _customerDataProvider.GetByRequestIdAsync(requestId);
        }

        public async Task<Customer> GetByInterventionIdAsync(int interventionId)
        {
            return await _customerDataProvider.GetByInterventionIdAsync(interventionId);
        }

        public async Task<Customer> GetByOfferIdAsync(int offerId)
        {
            return await _customerDataProvider.GetByOfferIdAsync(offerId);
        }

        public async Task<Customer> GetByOrderIdAsync(int orderId)
        {
            return await _customerDataProvider.GetByOrderIdAsync(orderId);
        }

        public async Task<Customer> GetByInvoiceIdAsync(int invoiceId)
        {
            return await _customerDataProvider.GetByInvoiceIdAsync(invoiceId);
        }

        public async Task<Customer> GetByDepositInvoiceIdAsync(int depositInvoiceId)
        {
            return await _customerDataProvider.GetByDepositInvoiceIdAsync(depositInvoiceId);
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            if (customer.DataId != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Customer cannot have a data-id on create.");
            if (customer.Id != 0 || customer.Number != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Customer cannot have an id/number on create.");
            if ((customer.PostalCode != null && customer.City == null) || (customer.PostalCode == null && customer.City != null))
                throw new IllegalArgumentException("IllegalAttribute", "Customer's postal-code and city must be both filled in or not filled.");
            if (!customer.IsValidVatNumber())
                throw new IllegalArgumentException("IllegalAttribute", "Invalid VAT number.");
            if (customer.Country == null)
                throw new IllegalArgumentException("IllegalAttribute", "Country is required on customer creation.");
            if (customer.Language == null)
                throw new IllegalArgumentException("IllegalAttribute", "Language is required on customer creation.");
            if (customer.Contacts != null || customer.Buildings != null)
            {
                var message = "Contacts, buildings, requests, offers, orders, deposit-invoices or invoices cannot be added to a customer on creation.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }

            if (string.IsNullOrEmpty(customer.Memo))
                customer.Memo = null;

            await EmbedRelations(customer);

            return await _customerDataProvider.CreateAsync(customer);
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "country", "language", "honorific-prefix", "tags" };
            var existingCustomer = await _customerDataProvider.GetByNumberAsync(customer.Id, query);

            if (customer.DataId != existingCustomer.DataId)
                throw new IllegalArgumentException("IllegalAttribute", "Customer data-id cannot be updated.");
            if (customer.Number != existingCustomer.Number || customer.Id != existingCustomer.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Customer id/number cannot be updated.");
            if ((customer.PostalCode != null && customer.City == null) || (customer.PostalCode == null && customer.City != null))
                throw new IllegalArgumentException("IllegalAttribute", "Customer's postal-code and city must be both filled in or not filled.");
            if (!customer.IsValidVatNumber())
                throw new IllegalArgumentException("IllegalAttribute", "Invalid VAT number.");
            if (customer.Country == null)
                throw new IllegalArgumentException("IllegalAttribute", "Country is required.");
            if (customer.Language == null)
                throw new IllegalArgumentException("IllegalAttribute", "Language is required.");

            if (string.IsNullOrEmpty(customer.Memo))
                customer.Memo = null;

            await EmbedRelations(customer, existingCustomer);

            return await _customerDataProvider.UpdateAsync(customer);
        }

        public async Task DeleteAsync(int id)
        {
            var query = new QuerySet();
            query.Page.Size = 1;

            var requests = await _requestDataProvider.GetAllByCustomerIdAsync(id, query);
            if (requests.Count > 0)
            {
                _logger.LogError($"Customer {id} cannot be deleted because requests are still attached to it.");
                throw new InvalidOperationException($"Customer {id} cannot be deleted because requests are still attached to it.");
            }

            var invoices = await _invoiceDataProvider.GetAllByCustomerIdAsync(id, query);
            if (invoices.Count > 0)
            {
                _logger.LogError($"Customer {id} cannot be deleted because invoices are still attached to it.");
                throw new InvalidOperationException($"Customer {id} cannot be deleted because invoices are still attached to it.");
            }

            var contacts = await _contactDataProvider.GetAllByCustomerIdAsync(id, query);
            if (contacts.Count > 0)
            {
                _logger.LogError($"Customer {id} cannot be deleted because contacts are still attached to it.");
                throw new InvalidOperationException($"Customer {id} cannot be deleted because contacts are still attached to it.");
            }

            var buildings = await _buildingDataProvider.GetAllByCustomerIdAsync(id, query);
            if (buildings.Count > 0)
            {
                _logger.LogError($"Customer {id} cannot be deleted because buildings are still attached to it.");
                throw new InvalidOperationException($"Customer {id} cannot be deleted because buildings are still attached to it.");
            }

            await _customerDataProvider.DeleteByNumberAsync(id);
        }

        // Embed relations in customer resource: reuse old relation if there is one and it hasn't changed
        private async Task EmbedRelations(Customer customer, Customer oldCustomer = null)
        {
            try {
                if (customer.Country != null)
                {
                    if (oldCustomer != null && oldCustomer.Country != null && oldCustomer.Country.Id == customer.Country.Id)
                        customer.Country = oldCustomer.Country;
                    else
                        customer.Country = await _countryDataProvider.GetByIdAsync(int.Parse(customer.Country.Id));
                }

                if (customer.Language != null)
                {
                    if (oldCustomer != null && oldCustomer.Language != null && oldCustomer.Language.Id == customer.Language.Id)
                        customer.Language = oldCustomer.Language;
                    else
                        customer.Language = await _langugageDataProvider.GetByIdAsync(int.Parse(customer.Language.Id));
                }

                if (customer.HonorificPrefix != null)
                {
                    if (oldCustomer != null && oldCustomer.HonorificPrefix != null && oldCustomer.HonorificPrefix.Id == customer.HonorificPrefix.Id)
                        customer.HonorificPrefix = oldCustomer.HonorificPrefix;
                    else
                        customer.HonorificPrefix = await _honorificPrefixDataProvider.GetByIdAsync(customer.HonorificPrefix.Id);
                }

                if (customer.Tags != null)
                {
                    if (oldCustomer != null && oldCustomer.Tags != null && Tag.Equals(customer.Tags, oldCustomer.Tags))
                        customer.Tags = oldCustomer.Tags;
                    else
                        customer.Tags = await Task.WhenAll(customer.Tags.Select(t => _tagDataProvider.GetByIdAsync(int.Parse(t.Id))));
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