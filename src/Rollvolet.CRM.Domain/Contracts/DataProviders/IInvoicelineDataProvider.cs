using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IInvoicelineDataProvider
    {
        Task<Invoiceline> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<Invoiceline>> GetAllByOrderIdAsync(int orderId, QuerySet query);
        Task<Paged<Invoiceline>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query);
        Task<Invoiceline> CreateAsync(Invoiceline invoiceline);
        Task<Invoiceline> UpdateAsync(Invoiceline invoiceline);
        Task DeleteByIdAsync(int id);
    }
}