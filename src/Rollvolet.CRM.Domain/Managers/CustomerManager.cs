using System;
using System.Collections.Generic;
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
        private readonly ILogger _logger;

        public CustomerManager(ICustomerDataProvider customerDataProvider, ICountryDataProvider countryDataProvider,
                                IHonorificPrefixDataProvider honorificPrefixDataProvider, ILanguageDataProvider languageDataProvider,
                                ITagDataProvider tagDataProvider, ILogger<CustomerManager> logger)
        {
            _customerDataProvider = customerDataProvider;
            _countryDataProvider = countryDataProvider;
            _honorificPrefixDataProvider = honorificPrefixDataProvider;
            _langugageDataProvider = languageDataProvider;
            _tagDataProvider = tagDataProvider;
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

        public async Task<Customer> CreateAsync(Customer customer)
        {
            // Validations
            if (customer.DataId != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Customer cannot have a data-id on create.");
            if (customer.Id != 0 || customer.Number != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Customer cannot have an id/number on create.");
            if ((customer.PostalCode != null && customer.City == null) || (customer.PostalCode == null && customer.City != null))
                throw new IllegalArgumentException("IllegalAttribute", "Customer's postal-code and city must be both filled in or not filled.");
            if (customer.Telephones != null || customer.Contacts != null || customer.Buildings != null || customer.Requests != null
                  || customer.Offers != null || customer.Orders != null || customer.DepositInvoices != null || customer.Invoices != null)
            {
                var message = "Telephones, contacts, buildings, requests, offers, orders, deposit-invoices or invoices cannot be added to a customer on creation.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }
            if (customer.Country == null)
                throw new IllegalArgumentException("IllegalAttribute", "Country is required on customer creation.");
            if (customer.Language == null)
                throw new IllegalArgumentException("IllegalAttribute", "Language is required on customer creation.");


            // Embed existing records
            try {
                var id = int.Parse(customer.Country.Id);
                customer.Country = await _countryDataProvider.GetByIdAsync(id);

                id = int.Parse(customer.Language.Id);
                customer.Language = await _langugageDataProvider.GetByIdAsync(id);

                if (customer.HonorificPrefix != null)
                {
                    var composedId = customer.HonorificPrefix.Id;
                    customer.HonorificPrefix = await _honorificPrefixDataProvider.GetByIdAsync(composedId);
                }
                if (customer.Tags != null)
                {
                    customer.Tags = await Task.WhenAll(customer.Tags.Select(t => {
                        id = int.Parse(t.Id);
                        return _tagDataProvider.GetByIdAsync(id);
                    }));
                }
            }
            catch (EntityNotFoundException) 
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }


            return await _customerDataProvider.CreateAsync(customer);
        }

        public async Task DeleteAsync(int id)
        {
            await _customerDataProvider.DeleteByNumberAsync(id);
        }
    }
}