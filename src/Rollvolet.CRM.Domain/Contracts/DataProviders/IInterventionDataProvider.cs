using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IInterventionDataProvider
    {
        Task<Paged<Intervention>> GetAllAsync(QuerySet query);
        Task<Intervention> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<Intervention>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Paged<Intervention>> GetAllByRelativeContactIdAsync (int customerId, int relativeContactId, QuerySet query);
        Task<Paged<Intervention>> GetAllByRelativeBuildingIdAsync(int customerId, int relativeBuildingId, QuerySet query);
        Task<Paged<Intervention>> GetAllByOrderIdAsync(int orderId, QuerySet query);
        Task<Intervention> GetByInvoiceIdAsync(int offerId);
        Task<Intervention> GetByFollowUpRequestIdAsync(int requestId);
        Task<Intervention> GetByPlanningEventIdAsync(int planningEventId);
        Task<Intervention> CreateAsync(Intervention intervention);
        Task<Intervention> UpdateAsync(Intervention intervention);
        Task<Intervention> UpdateContactAndBuildingAsync(int id, int? relativeContactId, int? relativeBuildingId);
        Task DeleteByIdAsync(int id);
    }
}