using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IInterventionManager
    {
        Task<Paged<Intervention>> GetAllAsync(QuerySet query);
        Task<Intervention> GetByIdAsync(int id, QuerySet query);
        Task<Paged<Intervention>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Paged<Intervention>> GetAllByContactIdAsync(int contactId, QuerySet query);
        Task<Paged<Intervention>> GetAllByBuildingIdAsync(int contactId, QuerySet query);
        Task<Paged<Intervention>> GetAllByOrderIdAsync(int orderId, QuerySet query);
        Task<Intervention> GetByInvoiceIdAsync(int invoiceId);
        Task<Intervention> GetByFollowUpRequestIdAsync(int requestId);
        Task<Intervention> CreateAsync(Intervention intervention);
        Task<Intervention> UpdateAsync(Intervention intervention);
        Task DeleteAsync(int id);
    }
}
