using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers
{
    public class PaymentManager : IPaymentManager
    {
        private readonly IPaymentDataProvider _paymentDataProvider;
        private readonly ILogger _logger;

        public PaymentManager(IPaymentDataProvider paymentDataProvider, ILogger<IPaymentDataProvider> logger)
        {
            _paymentDataProvider = paymentDataProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _paymentDataProvider.GetAll();
        }

        public async Task<Payment> GetByDepositIdAsync(int depositId)
        {
            return await _paymentDataProvider.GetByDepositIdAsync(depositId);
        }
    }
}