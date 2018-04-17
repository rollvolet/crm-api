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
    public class PostalCodeManager : IPostalCodeManager
    {
        private readonly IPostalCodeDataProvider _postalCodeDataProvider;
        private readonly ILogger _logger;

        public PostalCodeManager(IPostalCodeDataProvider postalCodeDataProvider, ILogger<PostalCodeManager> logger)
        {
            _postalCodeDataProvider = postalCodeDataProvider;
            _logger = logger;
        }
        
        public async Task<IEnumerable<PostalCode>> GetAllAsync()
        {
            return await _postalCodeDataProvider.GetAll();
        }
    }
}