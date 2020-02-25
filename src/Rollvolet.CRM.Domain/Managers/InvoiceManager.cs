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
    public class InvoiceManager : IInvoiceManager
    {
        private readonly IInvoiceDataProvider _invoiceDataProvider;
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly IContactDataProvider _contactDataProvider;
        private readonly IBuildingDataProvider _buildingDataProvider;
        private readonly IOrderDataProvider _orderDataProvider;
        private readonly IVatRateDataProvider _vatRateDataProvider;
        private readonly ILogger _logger;

        public InvoiceManager(IInvoiceDataProvider invoiceDataProvider, ICustomerDataProvider customerDataProvider,
                                IContactDataProvider contactDataProvider, IBuildingDataProvider buildingDataProvider,
                                IOrderDataProvider orderDataProvider, IVatRateDataProvider vatRateDataProvider,
                                 ILogger<InvoiceManager> logger)
        {
            _invoiceDataProvider = invoiceDataProvider;
            _customerDataProvider = customerDataProvider;
            _contactDataProvider = contactDataProvider;
            _buildingDataProvider = buildingDataProvider;
            _orderDataProvider = orderDataProvider;
            _vatRateDataProvider = vatRateDataProvider;
            _logger = logger;
        }

        public async Task<Paged<Invoice>> GetAllAsync(QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "number";
            }

            return await _invoiceDataProvider.GetAllAsync(query);
        }

        public async Task<Invoice> GetByIdAsync(int id, QuerySet query)
        {
            return await _invoiceDataProvider.GetByIdAsync(id, query);
        }

        public async Task<Paged<Invoice>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "number";
            }

            return await _invoiceDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }

        public async Task<Paged<Invoice>> GetAllByContactIdAsync(int contactId, QuerySet query)
        {
            try
            {
                var includeQuery = new QuerySet();
                includeQuery.Include.Fields = new string[] { "customer" };
                var contact = await _contactDataProvider.GetByIdAsync(contactId, includeQuery);

                if (query.Sort.Field == null)
                {
                    query.Sort.Order = SortQuery.ORDER_DESC;
                    query.Sort.Field = "number";
                }

                return await _invoiceDataProvider.GetAllByRelativeContactIdAsync(contact.Customer.Id, contact.Number, query);
            }
            catch (EntityNotFoundException)
            {
                return new Paged<Invoice> {
                    Count = 0,
                    Items = new List<Invoice>(),
                    PageNumber = 0,
                    PageSize = query.Page.Size
                };
            }
        }

        public async Task<Paged<Invoice>> GetAllByBuildingIdAsync(int buildingId, QuerySet query)
        {
            try
            {
                var includeQuery = new QuerySet();
                includeQuery.Include.Fields = new string[] { "customer" };
                var building = await _buildingDataProvider.GetByIdAsync(buildingId, includeQuery);

                if (query.Sort.Field == null)
                {
                    query.Sort.Order = SortQuery.ORDER_DESC;
                    query.Sort.Field = "number";
                }

                return await _invoiceDataProvider.GetAllByRelativeBuildingIdAsync(building.Customer.Id, building.Number, query);
            }
            catch (EntityNotFoundException)
            {
                return new Paged<Invoice> {
                    Count = 0,
                    Items = new List<Invoice>(),
                    PageNumber = 0,
                    PageSize = query.Page.Size
                };
            }
        }

        public async Task<Invoice> GetByOrderIdAsync(int orderId, QuerySet query = null)
        {
            return await _invoiceDataProvider.GetByOrderIdAsync(orderId, query);
        }

        public async Task<Invoice> GetByInterventionIdAsync(int interventionId, QuerySet query = null)
        {
            return await _invoiceDataProvider.GetByInterventionIdAsync(interventionId, query);
        }

        public async Task<Invoice> GetByInvoicelineIdAsync(int invoicelineId, QuerySet query = null)
        {
            return await _invoiceDataProvider.GetByInvoicelineIdAsync(invoicelineId);
        }

        public async Task<Invoice> GetByWorkingHourIdAsync(int workingHourId)
        {
            return await _invoiceDataProvider.GetByWorkingHourIdAsync(workingHourId);
        }

        public async Task<Invoice> CreateAsync(Invoice invoice)
        {
            if (invoice.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice cannot have an id on create.");
            if (invoice.Number != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice cannot have a number on create.");
            if (invoice.InvoiceDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice-date is required.");
            if (invoice.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required on invoice creation.");

            // Order cannot be required to support the creation of isolated invoices

            if (invoice.Order != null && invoice.VatRate == null)
                throw new IllegalArgumentException("IllegalAttribute", "VAT rate is required on non-isolated invoice creation.");

            // Embedded values cannot be set since they are not exposed in the DTO

            await EmbedRelationsAsync(invoice);

            if (invoice.Contact != null && invoice.Contact.Customer.Id != invoice.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Contact is not attached to customer {invoice.Contact.Id}.");
            if (invoice.Building != null && invoice.Building.Customer.Id != invoice.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Building is not attached to customer {invoice.Customer.Id}.");

            return await _invoiceDataProvider.CreateAsync(invoice);
        }

        public async Task<Invoice> UpdateAsync(Invoice invoice)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer", "building", "contact", "order", "vat-rate" };
            var existingInvoice = await _invoiceDataProvider.GetByIdAsync(invoice.Id, query);

            if (invoice.Id != existingInvoice.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice id cannot be updated.");
            if (invoice.Number != existingInvoice.Number)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice number cannot be updated.");
            if (invoice.InvoiceDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice-date is required.");

            // Embedded values cannot be updated since they are not exposed in the DTO

            await EmbedRelationsAsync(invoice, existingInvoice);

            if (invoice.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required.");
            if (invoice.Contact != null && invoice.Contact.Customer != null && invoice.Contact.Customer.Id != invoice.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Contact is not attached to customer {invoice.Contact.Id}.");
            if (invoice.Building != null && invoice.Building.Customer != null && invoice.Building.Customer.Id != invoice.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Building is not attached to customer {invoice.Customer.Id}.");

            // Order cannot be required to support the creation of isolated invoices

            if (invoice.Order != null
                    && existingInvoice.VatRate != null && invoice.VatRate != null && existingInvoice.VatRate.Id != invoice.VatRate.Id)
                throw new IllegalArgumentException("IllegalAttribute", "VAT rate of non-isolated invoice ccannot be updated.");

            return await _invoiceDataProvider.UpdateAsync(invoice);
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var invoice = await _invoiceDataProvider.GetByIdAsync(id);

                if (invoice.BookingDate != null)
                {
                    var message = $"Invoice {id} cannot be deleted because it has already been transferred to the accounting system.";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                await _invoiceDataProvider.DeleteByIdAsync(id);
            }
            catch (EntityNotFoundException)
            {
                // Invoice not found. Nothing should happen.
            }
        }

        // Embed relations in invoice resource: reuse old relation if there is one and it hasn't changed
        private async Task EmbedRelationsAsync(Invoice invoice, Invoice oldInvoice = null)
        {
            try {
                if (invoice.VatRate != null)
                {
                    if (oldInvoice != null && oldInvoice.VatRate != null && oldInvoice.VatRate.Id == invoice.VatRate.Id)
                        invoice.VatRate = oldInvoice.VatRate;
                    else
                        invoice.VatRate = await _vatRateDataProvider.GetByIdAsync(int.Parse(invoice.VatRate.Id));
                }

                // Customer cannot be updated. Take customer of oldRequest on update.
                if (oldInvoice != null)
                    invoice.Customer = oldInvoice.Customer;
                else
                    invoice.Customer = await _customerDataProvider.GetByNumberAsync(invoice.Customer.Id);

                // Order cannot be updated. Take order of oldRequest on update.
                if (oldInvoice != null)
                    invoice.Order = oldInvoice.Order;
                else if (invoice.Order != null) // isolated invoice doesn't have an order attached
                    invoice.Order = await _orderDataProvider.GetByIdAsync(invoice.Order.Id);

                var includeCustomer = new QuerySet();
                includeCustomer.Include.Fields = new string[] { "customer" };

                // Contact can only be updated through CaseManager. Take contact of oldInvoice on update.
                if (oldInvoice != null)
                    invoice.Contact = oldInvoice.Contact;
                else if (invoice.Contact != null)
                    invoice.Contact = await _contactDataProvider.GetByIdAsync(invoice.Contact.Id, includeCustomer);

                // Building can only be updated through CaseManager. Take building of oldInvoice on update.
                if (oldInvoice != null)
                    invoice.Building = oldInvoice.Building;
                else if (invoice.Building != null)
                    invoice.Building = await _buildingDataProvider.GetByIdAsync(invoice.Building.Id, includeCustomer);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }
    }
}