using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
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
        private readonly IPostalCodeDataProvider _postalCodeDataProvider;
        private readonly IContactDataProvider _contactDataProvider;
        private readonly ILogger _logger;

        public CustomerManager(ICustomerDataProvider customerDataProvider, ICountryDataProvider countryDataProvider,
                                IHonorificPrefixDataProvider honorificPrefixDataProvider, ILanguageDataProvider languageDataProvider,
                                IPostalCodeDataProvider postalCodeDataProvider, IContactDataProvider contactDataProvider,
                                ILogger<CustomerManager> logger)
        {
            _customerDataProvider = customerDataProvider;
            _countryDataProvider = countryDataProvider;
            _honorificPrefixDataProvider = honorificPrefixDataProvider;
            _langugageDataProvider = languageDataProvider;
            _postalCodeDataProvider = postalCodeDataProvider;
            _contactDataProvider = contactDataProvider;
            _logger = logger;
        }
        
        public async Task<Paged<Customer>> GetAllAsync(QuerySet query)
        {
            return await _customerDataProvider.GetAllAsync(query);
        }

        public async Task<Customer> GetByIdAsync(int id, QuerySet query)
        {
            return await _customerDataProvider.GetByNumberAsync(id, query);
        }

        public async Task<Customer> Create(Customer customer)
        {
            // TODO: replace related resources with the real objects
            if (customer.Country != null)
                customer.Country = await _countryDataProvider.GetByIdAsync(int.Parse(customer.Country.Id));
            if (customer.HonorificPrefix != null)
                customer.HonorificPrefix = await _honorificPrefixDataProvider.GetByIdAsync(int.Parse(customer.HonorificPrefix.Id));
            if (customer.Language != null)
                customer.Language = await _langugageDataProvider.GetByIdAsync(int.Parse(customer.Language.Id));
            if (customer.PostalCode != null)
                customer.PostalCode = await _postalCodeDataProvider.GetByIdAsync(int.Parse(customer.PostalCode.Id));

            if (customer.Contacts != null)
                customer.Contacts = customer.Contacts.Select(x => _contactDataProvider.GetByIdAsync(x.Id).Result);

            return await _customerDataProvider.Create(customer);
        }
    }
}