using System.Threading.Tasks;

namespace Rollvolet.CRM.Domain.Contracts.MsGraph
{
    public interface IGraphApiSystemTaskService
    {
        Task RenameOfferDocumentsAsync(string directory, int pageSize = 999);
    }
}