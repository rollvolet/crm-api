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

        public async Task<VatRate> GetByOfferIdAsync(int offerId)
        {
            return await _vatRateDataProvider.GetByOfferIdAsync(offerId);
        }

        public async Task<VatRate> GetByOrderIdAsync(int orderId)
        {
            return await _vatRateDataProvider.GetByOrderIdAsync(orderId);
        }

        public async Task<VatRate> GetByOfferlineIdAsync(int orderId)
        {
            return await _vatRateDataProvider.GetByOfferlineIdAsync(orderId);
        }

        public async Task<VatRate> GetByInvoiceIdAsync(int invoiceId)
        {
            return await _vatRateDataProvider.GetByInvoiceIdAsync(invoiceId);
        }

    }
}