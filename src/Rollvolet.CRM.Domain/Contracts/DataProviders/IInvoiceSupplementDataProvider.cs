using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IInvoiceSupplementDataProvider
    {
        Task<InvoiceSupplement> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<InvoiceSupplement>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query = null);
        Task<InvoiceSupplement> CreateAsync(InvoiceSupplement invoiceSupplement);
        Task<InvoiceSupplement> UpdateAsync(InvoiceSupplement invoiceSupplement);
        Task DeleteByIdAsync(int id);
    }
}