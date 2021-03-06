using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IRequestDataProvider
    {
        Task<Paged<Request>> GetAllAsync(QuerySet query);
        Task<Request> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<Request>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Paged<Request>> GetAllByRelativeContactIdAsync (int customerId, int relativeContactId, QuerySet query);
        Task<Paged<Request>> GetAllByRelativeBuildingIdAsync(int customerId, int relativeBuildingId, QuerySet query);
        Task<Request> GetByOfferIdAsync(int offerId, QuerySet query = null);
        Task<Request> GetByOrderIdAsync(int orderId, QuerySet query = null);
        Task<Request> GetByInterventionIdAsync(int interventionId);
        Task<Request> CreateAsync(Request request);
        Task<Request> UpdateAsync(Request request);
        Task<Request> UpdateContactAndBuildingAsync(int id, int? relativeContactId, int? relativeBuildingId);
        Task DeleteByIdAsync(int id);
    }
}