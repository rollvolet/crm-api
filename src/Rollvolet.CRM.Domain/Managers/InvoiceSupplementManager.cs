using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class InvoiceSupplementManager : IInvoiceSupplementManager
    {
        private readonly IInvoiceSupplementDataProvider _invoiceSupplementDataProvider;
        private readonly IInvoiceDataProvider _invoiceDataProvider;
        private readonly ILogger _logger;

        public InvoiceSupplementManager(IInvoiceSupplementDataProvider invoiceSupplementDataProvider, IInvoiceDataProvider invoiceDataProvider,
                                         ILogger<InvoiceManager> logger)
        {
            _invoiceSupplementDataProvider = invoiceSupplementDataProvider;
            _invoiceDataProvider = invoiceDataProvider;
            _logger = logger;
        }

        public async Task<Paged<InvoiceSupplement>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query)
        {
            return await _invoiceSupplementDataProvider.GetAllByInvoiceIdAsync(invoiceId, query);
        }

        public async Task<InvoiceSupplement> CreateAsync(InvoiceSupplement invoiceSupplement)
        {
            if (invoiceSupplement.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice-supplement cannot have an id on create.");
            if (invoiceSupplement.SequenceNumber != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice-supplement cannot have a sequence-number on create.");
            if (invoiceSupplement.Invoice == null)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice is required on deposit creation.");

            await EmbedRelationsAsync(invoiceSupplement);

            invoiceSupplement = await _invoiceSupplementDataProvider.CreateAsync(invoiceSupplement);

            await _invoiceDataProvider.SyncAmountAndVatAsync(invoiceSupplement.Invoice.Id);

            return invoiceSupplement;
        }

        public async Task<InvoiceSupplement> UpdateAsync(InvoiceSupplement invoiceSupplement)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "invoice" };
            var existingInvoiceSupplement = await _invoiceSupplementDataProvider.GetByIdAsync(invoiceSupplement.Id, query);

            if (invoiceSupplement.Id != existingInvoiceSupplement.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice-supplement id cannot be updated.");
            if (invoiceSupplement.SequenceNumber != existingInvoiceSupplement.SequenceNumber)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice-supplement sequence-number cannot be updated.");

            await EmbedRelationsAsync(invoiceSupplement, existingInvoiceSupplement);

            if (invoiceSupplement.Invoice == null)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice is required.");

            invoiceSupplement = await _invoiceSupplementDataProvider.UpdateAsync(invoiceSupplement);

            await _invoiceDataProvider.SyncAmountAndVatAsync(invoiceSupplement.Invoice.Id);

            return invoiceSupplement;
        }

        public async Task DeleteAsync(int id)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "invoice" };
            var invoiceSupplement = await _invoiceSupplementDataProvider.GetByIdAsync(id, query);
            var invoiceId = invoiceSupplement.Invoice.Id;

            await _invoiceSupplementDataProvider.DeleteByIdAsync(id);

            await _invoiceDataProvider.SyncAmountAndVatAsync(invoiceId);
        }

        // Embed relations in invoice supplement resource: reuse old relation if there is one and it hasn't changed
        private async Task EmbedRelationsAsync(InvoiceSupplement invoiceSupplement, InvoiceSupplement oldInvoiceSupplement = null)
        {
        try {
                if (oldInvoiceSupplement != null && oldInvoiceSupplement.Invoice != null)
                    invoiceSupplement.Invoice = oldInvoiceSupplement.Invoice; // frontend doesn't always include the invoice in PATCH requests
                else
                    invoiceSupplement.Invoice = await _invoiceDataProvider.GetByIdAsync(invoiceSupplement.Invoice.Id);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }
  }
}