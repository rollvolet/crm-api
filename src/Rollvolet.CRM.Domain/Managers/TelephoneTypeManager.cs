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
    public class TelephoneTypeManager : ITelephoneTypeManager
    {
        private readonly ITelephoneTypeDataProvider _telephoneTypeDataProvider;
        private readonly ILogger _logger;

        public TelephoneTypeManager(ITelephoneTypeDataProvider telephoneTypeDataProvider, ILogger<TelephoneTypeManager> logger)
        {
            _telephoneTypeDataProvider = telephoneTypeDataProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<TelephoneType>> GetAllAsync()
        {
            return await _telephoneTypeDataProvider.GetAllAsync();
        }

        public async Task<TelephoneType> GetByTelephoneIdAsync(string id)
        {
            return await _telephoneTypeDataProvider.GetByTelephoneIdAsync(id);
        }
    }
}