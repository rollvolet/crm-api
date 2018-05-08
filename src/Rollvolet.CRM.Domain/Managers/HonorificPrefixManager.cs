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
    public class HonorificPrefixManager : IHonorificPrefixManager
    {
        private readonly IHonorificPrefixDataProvider _honorificPrefixDataProvider;
        private readonly ILogger _logger;

        public HonorificPrefixManager(IHonorificPrefixDataProvider honorificPrefixDataProvider, ILogger<HonorificPrefixManager> logger)
        {
            _honorificPrefixDataProvider = honorificPrefixDataProvider;
            _logger = logger;
        }
        
        public async Task<IEnumerable<HonorificPrefix>> GetAllAsync()
        {
            return await _honorificPrefixDataProvider.GetAll();
        }
    }
}