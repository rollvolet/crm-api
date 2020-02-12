using System.Collections.Generic;
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
            return await _vatRateDataProvider.GetAllAsync();
        }

        public async Task<VatRate> GetByIdAsync(int id, QuerySet query = null)
        {
            return await _vatRateDataProvider.GetByIdAsync(id, query);
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

        public async Task<VatRate> GetByInvoicelineIdAsync(int invoicelineId)
        {
            return await _vatRateDataProvider.GetByInvoicelineIdAsync(invoicelineId);
        }

        public async Task<VatRate> GetByInvoiceIdAsync(int invoiceId)
        {
            return await _vatRateDataProvider.GetByInvoiceIdAsync(invoiceId);
        }

        public async Task<VatRate> GetByDepositInvoiceIdAsync(int depositInvoiceId)
        {
            return await _vatRateDataProvider.GetByDepositInvoiceIdAsync(depositInvoiceId);
        }

    }
}