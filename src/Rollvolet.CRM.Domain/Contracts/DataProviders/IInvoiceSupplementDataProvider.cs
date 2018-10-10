using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IInvoiceSupplementDataProvider
    {
        Task<Paged<InvoiceSupplement>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query);
        Task<InvoiceSupplement> CreateAsync(InvoiceSupplement invoiceSupplement);
        Task<InvoiceSupplement> UpdateAsync(InvoiceSupplement invoiceSupplement);
        Task DeleteByIdAsync(int id);
    }
}