using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IInvoiceSupplementManager
    {
        Task<Paged<InvoiceSupplement>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query);
        Task<InvoiceSupplement> CreateAsync(InvoiceSupplement invoiceSupplement);
        Task<InvoiceSupplement> UpdateAsync(InvoiceSupplement invoiceSupplement);
        Task DeleteAsync(int id);
    }
}
