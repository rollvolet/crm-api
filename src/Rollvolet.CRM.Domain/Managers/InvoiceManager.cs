using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Invoice> GetByOrderIdAsync(int orderId, QuerySet query = null)
        {
            return await _invoiceDataProvider.GetByOrderIdAsync(orderId, query);
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
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required on request creation.");

            // Order cannot be required to support the creation of isolated invoices

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

            // TODO trigger update reference of offer/order if reference changed
            // TODO trigger update reference of offer/order if vat-rate changed

            await EmbedRelationsAsync(invoice, existingInvoice);

            if (invoice.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required.");
            if (invoice.Contact != null && invoice.Contact.Customer.Id != invoice.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Contact is not attached to customer {invoice.Contact.Id}.");
            if (invoice.Building != null && invoice.Building.Customer.Id != invoice.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Building is not attached to customer {invoice.Customer.Id}.");

            // Order cannot be required to support the creation of isolated invoices

            return await _invoiceDataProvider.UpdateAsync(invoice);
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var invoice = await _invoiceDataProvider.GetByIdAsync(id);

                if (invoice.BookingDate != null)
                {
                    _logger.LogError($"Invoice {id} cannot be deleted because it has already been transfered to the accounting system.");
                    throw new InvalidOperationException($"Invoice {id} cannot be deleted because it has already been transfered to the accounting system.");
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

                // TODO prevent update of contact/building. Needs to bubble to request, order, etc.
                var includeCustomer = new QuerySet();
                includeCustomer.Include.Fields = new string[] { "customer" };
                if (invoice.Contact != null)
                {
                    if (oldInvoice != null && oldInvoice.Contact != null && oldInvoice.Contact.Id == invoice.Contact.Id)
                        invoice.Contact = oldInvoice.Contact;
                    else
                        invoice.Contact = await _contactDataProvider.GetByIdAsync(invoice.Contact.Id, includeCustomer);
                }

                if (invoice.Building != null)
                {
                    if (oldInvoice != null && oldInvoice.Building != null && oldInvoice.Building.Id == invoice.Building.Id)
                        invoice.Building = oldInvoice.Building;
                    else
                        invoice.Building = await _buildingDataProvider.GetByIdAsync(invoice.Building.Id, includeCustomer);
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
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }
    }
}