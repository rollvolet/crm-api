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
    public class VatRateManager : IVatRateManager
    {
        private readonly IVatRateDataProvider _vatRateDataProvider;
        private readonly ILogger _logger;

        public VatRateManager(IVatRateDataProvider vatRateDataProvider, ILogger<VatRateManager> logger)
        {
            _vatRateDataProvider = vatRateDataProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<VatRate>> GetAllAsync()
        {
            return await _vatRateDataProvider.GetAll();
        }

        public async Task<VatRate> GetByOfferIdAsync(int id)
        {
            return await _vatRateDataProvider.GetByOfferIdAsync(id);
        }
    }
}