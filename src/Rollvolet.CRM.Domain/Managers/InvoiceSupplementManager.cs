using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class InvoiceSupplementManager : IInvoiceSupplementManager
    {
        private readonly IInvoiceSupplementDataProvider _invoiceSupplementDataProvider;
        private readonly ILogger _logger;

        public InvoiceSupplementManager(IInvoiceSupplementDataProvider invoiceSupplementDataProvider, ILogger<InvoiceManager> logger)
        {
            _invoiceSupplementDataProvider = invoiceSupplementDataProvider;
            _logger = logger;
        }

        public async Task<Paged<InvoiceSupplement>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query)
        {
            return await _invoiceSupplementDataProvider.GetAllByInvoiceIdAsync(invoiceId, query);
        }

        public async Task<InvoiceSupplement> CreateAsync(InvoiceSupplement invoiceSupplement)
        {
            throw new System.NotImplementedException();
        }

        public async Task<InvoiceSupplement> UpdateAsync(InvoiceSupplement invoiceSupplement)
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }
  }
}