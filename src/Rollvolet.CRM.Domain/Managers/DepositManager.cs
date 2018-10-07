using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class DepositManager : IDepositManager
    {
        private readonly IDepositDataProvider _depositDataProvider;
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly IOrderDataProvider _orderDataProvider;
        private readonly IInvoiceDataProvider _invoiceDataProvider;
        private readonly IPaymentDataProvider _paymentDataProvider;
        private readonly ILogger _logger;

        public DepositManager(IDepositDataProvider depositDataProvider, ICustomerDataProvider customerDataProvider, IOrderDataProvider orderDataProvider,
                                IInvoiceDataProvider invoiceDataProvider, IPaymentDataProvider paymentDataProvider, ILogger<DepositManager> logger)
        {
            _depositDataProvider = depositDataProvider;
            _customerDataProvider = customerDataProvider;
            _orderDataProvider = orderDataProvider;
            _invoiceDataProvider = invoiceDataProvider;
            _paymentDataProvider = paymentDataProvider;
            _logger = logger;
        }

        public async Task<Paged<Deposit>> GetAllByOrderIdAsync(int orderId, QuerySet query)
        {
            if (query.Sort.Field == null) {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "payment-date";
            }

            return await _depositDataProvider.GetAllByOrderIdAsync(orderId, query);
        }

        public async Task<Paged<Deposit>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query)
        {
            if (query.Sort.Field == null) {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "payment-date";
            }

            return await _depositDataProvider.GetAllByInvoiceIdAsync(invoiceId, query);
        }

        public async Task<Deposit> CreateAsync(Deposit deposit)
        {
            if (deposit.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Deposit cannot have an id on create.");
            if (deposit.SequenceNumber != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Deposit cannot have a sequence-number on create.");
            if (deposit.PaymentDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Payment-date is required.");
            if (deposit.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required on deposit creation.");
            if (deposit.Order == null)
                throw new IllegalArgumentException("IllegalAttribute", "Order is required on deposit creation.");

            await EmbedRelations(deposit);

            if (deposit.Order.Customer.Id != deposit.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Order {deposit.Order.Id} isn't attached to customer {deposit.Customer.Id}.");

            return await _depositDataProvider.CreateAsync(deposit);
        }

        public async Task<Deposit> UpdateAsync(Deposit deposit)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer", "order" };
            var existingDeposit = await _depositDataProvider.GetByIdAsync(deposit.Id, query);

            if (deposit.Id != existingDeposit.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Deposit id cannot be updated.");
            if (deposit.SequenceNumber != existingDeposit.SequenceNumber)
                throw new IllegalArgumentException("IllegalAttribute", "Deposit sequence-number cannot be updated.");
            if (deposit.PaymentDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Payment-date is required.");

            await EmbedRelations(deposit, existingDeposit);

            if (deposit.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required.");
            if (deposit.Order == null)
                throw new IllegalArgumentException("IllegalAttribute", "Order is required.");

            return await _depositDataProvider.UpdateAsync(deposit);
        }

        public async Task DeleteAsync(int id)
        {
            await _depositDataProvider.DeleteByIdAsync(id);
        }

        // Embed relations in request resource: reuse old relation if there is one and it hasn't changed
        private async Task EmbedRelations(Deposit deposit, Deposit oldDeposit = null)
        {
            try {
                if (deposit.Invoice != null)
                {
                    if (oldDeposit != null && oldDeposit.Invoice != null && oldDeposit.Invoice.Id == deposit.Invoice.Id)
                        deposit.Invoice = oldDeposit.Invoice;
                    else
                        deposit.Invoice = await _invoiceDataProvider.GetByIdAsync(deposit.Invoice.Id);
                }

                if (deposit.Payment != null)
                {
                    if (oldDeposit != null && oldDeposit.Payment != null && oldDeposit.Payment.Id == deposit.Payment.Id)
                        deposit.Payment = oldDeposit.Payment;
                    else
                        deposit.Payment = await _paymentDataProvider.GetByIdAsync(deposit.Payment.Id);
                }

                // Customer cannot be updated. Take customer of oldDeposit on update.
                if (oldDeposit != null)
                    deposit.Customer = oldDeposit.Customer;
                else
                    deposit.Customer = await _customerDataProvider.GetByNumberAsync(deposit.Customer.Id);

                // Order cannot be updated. Take order of oldDeposit on update.
                var includeCustomer = new QuerySet();
                includeCustomer.Include.Fields = new string[] { "customer" };
                if (oldDeposit != null)
                    deposit.Order = oldDeposit.Order;
                else
                    deposit.Order = await _orderDataProvider.GetByIdAsync(deposit.Order.Id, includeCustomer);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }
    }
}