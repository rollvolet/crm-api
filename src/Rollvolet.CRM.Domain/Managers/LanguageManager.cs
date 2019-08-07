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
    public class LanguageManager : ILanguageManager
    {
        private readonly ILanguageDataProvider _languageDataProvider;
        private readonly ILogger _logger;

        public LanguageManager(ILanguageDataProvider languageDataProvider, ILogger<LanguageManager> logger)
        {
            _languageDataProvider = languageDataProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<Language>> GetAllAsync()
        {
            return await _languageDataProvider.GetAllAsync();
        }

        public async Task<Language> GetByCustomerIdAsync(int id)
        {
            return await _languageDataProvider.GetByCustomerNumberAsync(id);
        }

        public async Task<Language> GetByContactIdAsync(int id)
        {
            return await _languageDataProvider.GetByContactIdAsync(id);
        }

        public async Task<Language> GetByBuildingIdAsync(int id)
        {
            return await _languageDataProvider.GetByBuildingIdAsync(id);
        }
    }
}