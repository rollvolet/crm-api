using System.Threading.Tasks;

namespace Rollvolet.CRM.Domain.Contracts.MsGraph
{
    public interface IGraphApiService
    {
        Task<object> GetUserProfileAsync();
    }
}