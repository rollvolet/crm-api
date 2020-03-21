using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Contracts.MsGraph;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class OrderManager : IOrderManager
    {
        private readonly IOrderDataProvider _orderDataProvider;
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly IContactDataProvider _contactDataProvider;
        private readonly IBuildingDataProvider _buildingDataProvider;
        private readonly IOfferDataProvider _offerDataProvider;
        private readonly IInvoiceDataProvider _invoiceDataProvider;
        private readonly IInvoicelineDataProvider _invoicelineDataProvider;
        private readonly IDepositDataProvider _depositDataProvider;
        private readonly IDepositInvoiceDataProvider _depositInvoiceDataProvider;
        private readonly IVatRateDataProvider _vatRateDataProvider;
        private readonly IGraphApiService _graphApiService;
        private readonly ILogger _logger;

        public OrderManager(IOrderDataProvider orderDataProvider, IInvoiceDataProvider invoiceDataProvider,
                                ICustomerDataProvider customerDataProvider, IContactDataProvider contactDataProvider,
                                IBuildingDataProvider buildingDataProvider, IOfferDataProvider offerDataProvider,
                                IInvoicelineDataProvider invoicelineDataProvider, IVatRateDataProvider vatRateDataProvider,
                                IDepositDataProvider depositDataProvider, IDepositInvoiceDataProvider depositInvoiceDataProvider,
                                IGraphApiService graphApiService, ILogger<OrderManager> logger)
        {
            _orderDataProvider = orderDataProvider;
            _customerDataProvider = customerDataProvider;
            _contactDataProvider = contactDataProvider;
            _buildingDataProvider = buildingDataProvider;
            _offerDataProvider = offerDataProvider;
            _invoiceDataProvider = invoiceDataProvider;
            _invoicelineDataProvider = invoicelineDataProvider;
            _depositDataProvider = depositDataProvider;
            _depositInvoiceDataProvider = depositInvoiceDataProvider;
            _vatRateDataProvider = vatRateDataProvider;
            _graphApiService = graphApiService;
            _logger = logger;
        }

        public async Task<Paged<Order>> GetAllAsync(QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "order-date";
            }

            return await _orderDataProvider.GetAllAsync(query);
        }

        public async Task<Order> GetByIdAsync(int id, QuerySet query = null)
        {
            return await _orderDataProvider.GetByIdAsync(id, query);
        }

        public async Task<Paged<Order>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "order-date";
            }

            return await _orderDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }

        public async Task<Order> GetByInterventionIdAsync(int interventionId, QuerySet query = null)
        {
            return await _orderDataProvider.GetByInterventionIdAsync(interventionId, query);
        }

        public async Task<Order> GetByOfferIdAsync(int offerId, QuerySet query = null)
        {
            return await _orderDataProvider.GetByOfferIdAsync(offerId, query);
        }

        public async Task<Order> GetByInvoiceIdAsync(int invoiceId, QuerySet query = null)
        {
            return await _orderDataProvider.GetByInvoiceIdAsync(invoiceId, query);
        }

        public async Task<Order> GetByInvoicelineIdAsync(int invoicelineId, QuerySet query = null)
        {
            return await _orderDataProvider.GetByInvoicelineIdAsync(invoicelineId);
        }

        public async Task<Order> GetByDepositInvoiceIdAsync(int depositInvoiceId, QuerySet query = null)
        {
            return await _orderDataProvider.GetByDepositInvoiceIdAsync(depositInvoiceId, query);
        }


        public async Task<Order> CreateAsync(Order order)
        {
            if (order.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Order cannot have an id on create.");
            if (order.OrderDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Order-date is required.");
            if (order.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required on order creation.");
            if (order.Offer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Offer is required on order creation.");
            if (order.Invoice != null)
            {
                var message = "Invoice cannot be added to an order on creation.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }
            if (order.PlanningMsObjectId != null)
                throw new IllegalArgumentException("IllegalAttribute", "Calendar properties cannot be set.");

            await EmbedRelations(order);

            if (order.Contact != null && order.Contact.Customer.Id != order.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Contact is not attached to customer {order.Contact.Id}.");
            if (order.Building != null && order.Building.Customer.Id != order.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Building is not attached to customer {order.Customer.Id}.");

            if (order.PlanningDate != null)
                await SyncPlanningEventAsync(order);

            return await _orderDataProvider.CreateAsync(order);
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer", "building", "contact", "offer", "vat-rate" };
            var existingOrder = await _orderDataProvider.GetByIdAsync(order.Id, query);

            if (order.Id != existingOrder.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Order id cannot be updated.");
            if (order.OfferNumber != existingOrder.OfferNumber)
                throw new IllegalArgumentException("IllegalAttribute", "Offer number cannot be updated.");
            if (order.RequestNumber != existingOrder.RequestNumber)
                throw new IllegalArgumentException("IllegalAttribute", "Request number cannot be updated.");
            if (order.OrderDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Order-date is required.");
            if (order.PlanningMsObjectId != existingOrder.PlanningMsObjectId)
                throw new IllegalArgumentException("IllegalAttribute", "Calendar properties cannot be updated.");

            await EmbedRelations(order, existingOrder);

            if (order.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required.");
            if (order.Offer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Offer is required.");
            if (order.Contact != null && order.Contact.Customer != null && order.Contact.Customer.Id != order.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Contact is not attached to customer {order.Contact.Id}.");
            if (order.Building != null && order.Building.Customer != null && order.Building.Customer.Id != order.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Building is not attached to customer {order.Customer.Id}.");

            var requiresUpdate = RequiresPlanningEventUpdate(existingOrder, order);
            var requiresReschedule = existingOrder.PlanningDate != order.PlanningDate;
            await SyncPlanningEventAsync(order, requiresUpdate, requiresReschedule);

            return await _orderDataProvider.UpdateAsync(order);
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var invoice = await _invoiceDataProvider.GetByOrderIdAsync(id);
                var message = $"Order {id} cannot be deleted because invoice {invoice.Id} is attached to it.";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }
            catch(EntityNotFoundException)
            {
                var pageQuery = new QuerySet();
                pageQuery.Page.Size = 1;

                var depositInvoices = await _depositInvoiceDataProvider.GetAllByOrderIdAsync(id, pageQuery);
                if (depositInvoices.Count > 0)
                {
                    var message = $"Order {id} cannot be deleted because {depositInvoices.Count} deposit-invoices attached to it.";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                var deposits = await _depositDataProvider.GetAllByOrderIdAsync(id, pageQuery);
                if (deposits.Count > 0)
                {
                    var message = $"Order {id} cannot be deleted because {deposits.Count} deposits are attached to it.";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                var invoicelines = await _invoicelineDataProvider.GetAllByOrderIdAsync(id, pageQuery);
                if (invoicelines.Count > 0)
                {
                    var message = $"Order {id} cannot be deleted because {invoicelines.Count} invoicelines are attached to it.";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                try
                {
                    var order = await _orderDataProvider.GetByIdAsync(id);
                    await _graphApiService.DeleteEventForPlanningAsync(order);
                    await _orderDataProvider.DeleteByIdAsync(id);
                }
                catch (EntityNotFoundException)
                {
                    // Order not found. Nothing should happen.
                }
            }
        }

        public async Task SyncPlanningEventAsync(Order order, bool requiresUpdate = false, bool requiresReschedule = false)
        {
            if (order.PlanningMsObjectId != null && order.PlanningDate == null)
            {
                await _graphApiService.DeleteEventForPlanningAsync(order);
            }
            else if (order.PlanningMsObjectId != null && requiresUpdate)
            {
                await _graphApiService.UpdateEventForPlanningAsync(order, requiresReschedule);
            }
            else if (order.PlanningMsObjectId == null && String.IsNullOrEmpty(order.PlanningId) && order.PlanningDate != null)
            {
                await _graphApiService.CreateEventForPlanningAsync(order);
            }
        }

        public async Task SyncPlanningEventAsync(int orderId, bool requiresUpdate = false)
        {
            try
            {
                var query = new QuerySet();
                query.Include.Fields = new string[] { "customer", "building", "contact" };
                var order = await GetByIdAsync(orderId, query);
                var requiresReschedule = false; // Update triggered by change outside the order, so order.PlanningDate didn't change
                await SyncPlanningEventAsync(order, requiresUpdate, requiresReschedule);
                await _orderDataProvider.UpdateAsync(order);
            }
            catch (EntityNotFoundException)
            {
                // No calendar event to update
            }
        }

        // Embed relations in request resource: reuse old relation if there is one and it hasn't changed
        private async Task EmbedRelations(Order order, Order oldOrder = null)
        {
            try {
                if (order.VatRate != null)
                {
                    if (oldOrder != null && oldOrder.VatRate != null && oldOrder.VatRate.Id == order.VatRate.Id)
                        order.VatRate = oldOrder.VatRate;
                    else
                        order.VatRate = await _vatRateDataProvider.GetByIdAsync(int.Parse(order.VatRate.Id));
                }

                // Customer cannot be updated. Take customer of oldOrder on update.
                if (oldOrder != null)
                    order.Customer = oldOrder.Customer;
                else
                    order.Customer = await _customerDataProvider.GetByNumberAsync(order.Customer.Id);

                // Offer cannot be updated. Take offer of oldOrder on update.
                if (oldOrder != null)
                    order.Offer = oldOrder.Offer;
                else
                    order.Offer = await _offerDataProvider.GetByIdAsync(order.Offer.Id);

                // Invoice cannot be updated. Take invoice of oldOrder on update.
                if (oldOrder != null)
                    order.Invoice = oldOrder.Invoice;
                else
                    order.Invoice = null;

                var includeCustomer = new QuerySet();
                includeCustomer.Include.Fields = new string[] { "customer" };

                // Contact can only be updated through CaseManager. Take contact of oldOrder on update.
                if (oldOrder != null)
                    order.Contact = oldOrder.Contact;
                else if (order.Contact != null)
                    order.Contact = await _contactDataProvider.GetByIdAsync(order.Contact.Id, includeCustomer);

                // Building can only be updated through CaseManager. Take building of oldOrder on update.
                if (oldOrder != null)
                    order.Building = oldOrder.Building;
                else if (order.Building != null)
                    order.Building = await _buildingDataProvider.GetByIdAsync(order.Building.Id, includeCustomer);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }

        private bool RequiresPlanningEventUpdate(Order existingOrder, Order order)
        {
            return order.PlanningDate != existingOrder.PlanningDate
              || order.ScheduledHours != existingOrder.ScheduledHours
              || order.ScheduledNbOfPersons != existingOrder.ScheduledNbOfPersons
              || order.MustBeDelivered != existingOrder.MustBeDelivered;
        }
    }
}