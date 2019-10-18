using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface ICaseManager
    {
        Task<Case> GetCaseAsync(int? requestId, int? offerId, int? orderId, int? invoiceId);
        Task UpdateContactAndBuildingAsync(int? contactId, int? buildingId, int? requestId, int? offerId, int? orderId, int? invoiceId);
    }
}
