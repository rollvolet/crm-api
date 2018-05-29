using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers
{
    public class VisitManager : IVisitManager
    {
        private readonly IVisitDataProvider _visitDataProvider;
        private readonly ILogger _logger;

        public VisitManager(IVisitDataProvider visitDataProvider, ILogger<VisitManager> logger)
        {
            _visitDataProvider = visitDataProvider;
            _logger = logger;
        }

        public async Task<Visit> GetByRequestIdAsync(int id)
        {
            return await _visitDataProvider.GetByRequestIdAsync(id);
        }
    }
}