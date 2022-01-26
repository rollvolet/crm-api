using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class DepositInvoiceManager : IDepositInvoiceManager
    {
        private readonly IDepositInvoiceDataProvider _depositInvoiceDataProvider;
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly IContactDataProvider _contactDataProvider;
        private readonly IBuildingDataProvider _buildingDataProvider;
        private readonly IOrderDataProvider _orderDataProvider;
        private readonly IVatRateDataProvider _vatRateDataProvider;
        private readonly IInvoiceDataProvider _invoiceDataProvider;
        private readonly IDocumentGenerationManager _documentGenerationManager;
        private readonly ILogger _logger;

        public DepositInvoiceManager(IDepositInvoiceDataProvider depositInvoiceDataProvider, ICustomerDataProvider customerDataProvider,
                                IContactDataProvider contactDataProvider, IBuildingDataProvider buildingDataProvider,
                                IOrderDataProvider orderDataProvider, IVatRateDataProvider vatRateDataProvider,
                                IInvoiceDataProvider invoiceDataProvider, IDocumentGenerationManager documentGenerationManager,
                                ILogger<InvoiceManager> logger)
        {
            _depositInvoiceDataProvider = depositInvoiceDataProvider;
            _customerDataProvider = customerDataProvider;
            _contactDataProvider = contactDataProvider;
            _buildingDataProvider = buildingDataProvider;
            _orderDataProvider = orderDataProvider;
            _invoiceDataProvider = invoiceDataProvider;
            _vatRateDataProvider = vatRateDataProvider;
            _documentGenerationManager = documentGenerationManager;
            _logger = logger;
        }

        public async Task<Paged<DepositInvoice>> GetAllAsync(QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "number";
            }

            return await _depositInvoiceDataProvider.GetAllAsync(query);
        }

        public async Task<DepositInvoice> GetByIdAsync(int id, QuerySet query)
        {
            return await _depositInvoiceDataProvider.GetByIdAsync(id, query);
        }

        public async Task<Paged<DepositInvoice>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "number";
            }

            return await _depositInvoiceDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }

        public async Task<Paged<DepositInvoice>> GetAllByOrderIdAsync(int orderId, QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "number";
            }

            return await _depositInvoiceDataProvider.GetAllByOrderIdAsync(orderId, query);
        }

        public async Task<Paged<DepositInvoice>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query)
        {
            if (query.Sort.Field == null) {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "payment-date";
            }

            return await _depositInvoiceDataProvider.GetAllByInvoiceIdAsync(invoiceId, query);
        }

        public async Task<DepositInvoice> CreateAsync(DepositInvoice depositInvoice)
        {
            if (depositInvoice.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Deposit-invoice cannot have an id on create.");
            if (depositInvoice.Number != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Deposit-invoice cannot have a number on create.");
            if (depositInvoice.BaseAmount == null)
                throw new IllegalArgumentException("IllegalAttribute", "Base amount is required on deposit-invoice.");
            if (depositInvoice.InvoiceDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Deposit-Invoice-date is required.");
            if (depositInvoice.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required on deposit invoice creation.");
            if (depositInvoice.Order == null)
                throw new IllegalArgumentException("IllegalAttribute", "Order is required on deposit invoice creation.");

            // Embedded values cannot be set since they are not exposed in the DTO

            await EmbedRelationsAsync(depositInvoice);

            if (depositInvoice.Contact != null && depositInvoice.Contact.Customer.Id != depositInvoice.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Contact is not attached to customer {depositInvoice.Contact.Id}.");
            if (depositInvoice.Building != null && depositInvoice.Building.Customer.Id != depositInvoice.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Building is not attached to customer {depositInvoice.Customer.Id}.");

            return await _depositInvoiceDataProvider.CreateAsync(depositInvoice);
        }

        public async Task<DepositInvoice> UpdateAsync(DepositInvoice depositInvoice)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer", "order", "building", "contact", "order", "vat-rate" };
            var existingDepositInvoice = await _depositInvoiceDataProvider.GetByIdAsync(depositInvoice.Id, query);

            if (depositInvoice.Id != existingDepositInvoice.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice id cannot be updated.");
            if (depositInvoice.Number != existingDepositInvoice.Number)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice number cannot be updated.");
            if (depositInvoice.BaseAmount == null)
                throw new IllegalArgumentException("IllegalAttribute", "Base amount is required on deposit-invoice.");
            if (depositInvoice.InvoiceDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice-date is required.");

            // Embedded values cannot be updated since they are not exposed in the DTO

            await EmbedRelationsAsync(depositInvoice, existingDepositInvoice);

            if (depositInvoice.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required.");
            if (depositInvoice.Order == null)
                throw new IllegalArgumentException("IllegalAttribute", "Order is required.");
            if (depositInvoice.Contact != null && depositInvoice.Contact.Customer != null
                && depositInvoice.Contact.Customer.Id != depositInvoice.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Contact is not attached to customer {depositInvoice.Contact.Id}.");
            if (depositInvoice.Building != null && depositInvoice.Building.Customer != null
                && depositInvoice.Building.Customer.Id != depositInvoice.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Building is not attached to customer {depositInvoice.Customer.Id}.");

            return await _depositInvoiceDataProvider.UpdateAsync(depositInvoice);
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var includeQuery = new QuerySet();
                includeQuery.Include.Fields = new string[] { "order" };
                var depositInvoice = await _depositInvoiceDataProvider.GetByIdAsync(id, includeQuery);

                if (depositInvoice.BookingDate != null)
                {
                    var message = $"Deposit-invoice {id} cannot be deleted because it has already been transferred to the accounting system.";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                try
                {
                    var invoice = await _invoiceDataProvider.GetByOrderIdAsync(depositInvoice.Order.Id);
                    var message = $"Deposit-invoice {id} cannot be deleted because invoice {invoice.Id} is attached to it.";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }
                catch(EntityNotFoundException)
                {
                    await _documentGenerationManager.DeleteDepositInvoiceDocumentAsync(id);
                    await _depositInvoiceDataProvider.DeleteByIdAsync(id);
                }
            }
            catch (EntityNotFoundException)
            {
                // Deposit invoice not found. Nothing should happen.
            }
        }

        // Embed relations in deposit invoice resource: reuse old relation if there is one and it hasn't changed
        private async Task EmbedRelationsAsync(DepositInvoice depositInvoice, DepositInvoice oldDepositInvoice = null)
        {
            try {
                if (depositInvoice.VatRate != null)
                {
                    if (oldDepositInvoice != null && oldDepositInvoice.VatRate != null && oldDepositInvoice.VatRate.Id == depositInvoice.VatRate.Id)
                        depositInvoice.VatRate = oldDepositInvoice.VatRate;
                    else
                        depositInvoice.VatRate = await _vatRateDataProvider.GetByIdAsync(int.Parse(depositInvoice.VatRate.Id));
                }

                // Customer cannot be updated. Take customer of oldDepositInvoice on update.
                if (oldDepositInvoice != null)
                    depositInvoice.Customer = oldDepositInvoice.Customer;
                else
                    depositInvoice.Customer = await _customerDataProvider.GetByNumberAsync(depositInvoice.Customer.Id);

                // Order cannot be updated. Take order of oldDepositInvoice on update.
                if (oldDepositInvoice != null)
                    depositInvoice.Order = oldDepositInvoice.Order;
                else if (depositInvoice.Order != null) // isolated invoice doesn't have an order attached
                    depositInvoice.Order = await _orderDataProvider.GetByIdAsync(depositInvoice.Order.Id);

                var includeCustomer = new QuerySet();
                includeCustomer.Include.Fields = new string[] { "customer" };

                // Contact can only be updated through CaseManager. Take contact of oldDepositInvoice on update.
                if (oldDepositInvoice != null)
                    depositInvoice.Contact = oldDepositInvoice.Contact;
                else if (depositInvoice.Contact != null)
                    depositInvoice.Contact = await _contactDataProvider.GetByIdAsync(depositInvoice.Contact.Id, includeCustomer);

                // Building can only be updated through CaseManager. Take building of oldDepositInvoice on update.
                if (oldDepositInvoice != null)
                    depositInvoice.Building = oldDepositInvoice.Building;
                else if (depositInvoice.Building != null)
                    depositInvoice.Building = await _buildingDataProvider.GetByIdAsync(depositInvoice.Building.Id, includeCustomer);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }

    }
}