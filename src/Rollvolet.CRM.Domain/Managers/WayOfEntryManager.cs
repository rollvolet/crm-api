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
    public class WayOfEntryManager : IWayOfEntryManager
    {
        private readonly IWayOfEntryDataProvider _wayOfEntryDataProvider;
        private readonly ILogger _logger;

        public WayOfEntryManager(IWayOfEntryDataProvider wayOfEntryDataProvider, ILogger<WayOfEntryManager> logger)
        {
            _wayOfEntryDataProvider = wayOfEntryDataProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<WayOfEntry>> GetAllAsync()
        {
            return await _wayOfEntryDataProvider.GetAll();
        }
    }
}