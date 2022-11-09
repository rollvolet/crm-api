using System.Threading.Tasks;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface ICaseManager
    {
        Task UpdateContactAndBuildingAsync(int? contactId, int? buildingId, int? requestId, int? interventionId, int? offerId, int? orderId, int? invoiceId);
    }
}
