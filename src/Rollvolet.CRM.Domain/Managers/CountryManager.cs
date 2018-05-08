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
    public class CountryManager : ICountryManager
    {
        private readonly ICountryDataProvider _countryDataProvider;
        private readonly ILogger _logger;

        public CountryManager(ICountryDataProvider countryDataProvider, ILogger<CountryManager> logger)
        {
            _countryDataProvider = countryDataProvider;
            _logger = logger;
        }
        
        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            return await _countryDataProvider.GetAll();
        }
    }
}