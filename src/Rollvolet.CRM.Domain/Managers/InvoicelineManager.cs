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
    public class InvoicelineManager : IInvoicelineManager
    {
        private readonly IInvoicelineDataProvider _invoicelineDataProvider;
        private readonly IOrderDataProvider _orderDataProvider;
        private readonly IInvoiceDataProvider _invoiceDataProvider;
        private readonly IVatRateDataProvider _vatRateDataProvider;
        private readonly ILogger _logger;

        public InvoicelineManager(IInvoicelineDataProvider invoicelineDataProvider, IOrderDataProvider orderDataProvider,
                                IInvoiceDataProvider invoiceDataProvider, IVatRateDataProvider vatRateDataProvider,
                                ILogger<InvoiceManager> logger)
        {
            _invoicelineDataProvider = invoicelineDataProvider;
            _orderDataProvider = orderDataProvider;
            _invoiceDataProvider = invoiceDataProvider;
            _vatRateDataProvider = vatRateDataProvider;
            _logger = logger;
        }

        public async Task<Invoiceline> GetByIdAsync(int id, QuerySet query)
        {
            return await _invoicelineDataProvider.GetByIdAsync(id, query);
        }

        public async Task<Paged<Invoiceline>> GetAllByOrderIdAsync(int orderId, QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_ASC;
                query.Sort.Field = "sequence-number";
            }

            return await _invoicelineDataProvider.GetAllByOrderIdAsync(orderId, query);
        }

        public async Task<Paged<Invoiceline>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_ASC;
                query.Sort.Field = "sequence-number";
            }

            return await _invoicelineDataProvider.GetAllByInvoiceIdAsync(invoiceId, query);
        }

        public async Task<Invoiceline> CreateAsync(Invoiceline invoiceline)
        {
            if (invoiceline.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Invoiceline cannot have an id on create.");
            if (invoiceline.Order == null && invoiceline.Invoice == null)
                throw new IllegalArgumentException("IllegalAttribute", "Order or invoice is required on invoiceline creation.");
            if (invoiceline.Amount == null)
                throw new IllegalArgumentException("IllegalAttribute", "Amount is required on invoiceline creation.");

            await EmbedRelations(invoiceline);

            if (invoiceline.Order == null && invoiceline.Invoice == null)
                throw new IllegalArgumentException("IllegalAttribute", "Order or invoice is required.");
            if (invoiceline.VatRate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Vat-rate is required.");

            return await _invoicelineDataProvider.CreateAsync(invoiceline);
        }

        public async Task<Invoiceline> UpdateAsync(Invoiceline invoiceline)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "order", "invoice", "vat-rate" };
            var existingInvoiceline = await _invoicelineDataProvider.GetByIdAsync(invoiceline.Id, query);

            if (invoiceline.Id != existingInvoiceline.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Invoiceline id cannot be updated.");
            if (invoiceline.Amount == null)
                throw new IllegalArgumentException("IllegalAttribute", "Amount is required on invoiceline.");

            await EmbedRelations(invoiceline, existingInvoiceline);

            if (invoiceline.Order == null && invoiceline.Invoice == null)
                throw new IllegalArgumentException("IllegalAttribute", "Order or invoice is required.");
            if (invoiceline.VatRate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Vat-rate is required.");

            return await _invoicelineDataProvider.UpdateAsync(invoiceline);
        }

        public async Task DeleteAsync(int id)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "invoice" };
            var invoiceline = await _invoicelineDataProvider.GetByIdAsync(id, query);

            try
            {
                if (invoiceline.Invoice != null)
                {
                    var invoice = await _invoiceDataProvider.GetByIdAsync(invoiceline.Invoice.Id);

                    if (invoice.BookingDate != null)
                    {
                        var message = $"Invoiceline {id} cannot be deleted because the invoice that is attached to it has already been transferred to the accounting system.";
                        _logger.LogError(message);
                        throw new InvalidOperationException(message);
                    }
                }
            }
            catch(EntityNotFoundException)
            {
                // No invoice found. Invoiceline can be removed
            }

            await _invoicelineDataProvider.DeleteByIdAsync(id);

        }

        // Embed relations in invoiceline resource: reuse old relation if there is one and it hasn't changed
        private async Task EmbedRelations(Invoiceline invoiceline, Invoiceline oldInvoiceline = null)
        {
            try {
                if (invoiceline.VatRate != null)
                {
                    if (oldInvoiceline != null && oldInvoiceline.VatRate != null && oldInvoiceline.VatRate.Id == invoiceline.VatRate.Id)
                        invoiceline.VatRate = oldInvoiceline.VatRate;
                    else
                        invoiceline.VatRate = await _vatRateDataProvider.GetByIdAsync(int.Parse(invoiceline.VatRate.Id));
                }

                // Order cannot be updated. Take Order of oldInvoiceline on update.
                if (oldInvoiceline != null)
                    invoiceline.Order = oldInvoiceline.Order;
                else
                    invoiceline.Order = await _orderDataProvider.GetByIdAsync(invoiceline.Order.Id);

                if (invoiceline.Invoice != null)
                {
                    if (oldInvoiceline != null && oldInvoiceline.Invoice != null && oldInvoiceline.Invoice.Id == invoiceline.Invoice.Id)
                        invoiceline.Invoice = oldInvoiceline.Invoice;
                    else
                        invoiceline.Invoice = await _invoiceDataProvider.GetByIdAsync(invoiceline.Invoice.Id);
                }
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }
    }
}