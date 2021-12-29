using System.Threading.Tasks;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface ISystemTaskManager
    {
        Task RecalculateSearchNames();
        Task RenameOfferDocuments();
    }
}