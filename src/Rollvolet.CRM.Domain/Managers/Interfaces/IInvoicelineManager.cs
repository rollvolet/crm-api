using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IInvoicelineManager
    {
        Task<Invoiceline> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<Invoiceline>> GetAllByOrderIdAsync(int orderId, QuerySet query);
        Task<Paged<Invoiceline>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query);
        Task<Invoiceline> CreateAsync(Invoiceline invoiceline);
        Task<Invoiceline> UpdateAsync(Invoiceline invoiceline);
        Task DeleteAsync(int id);
    }
}
