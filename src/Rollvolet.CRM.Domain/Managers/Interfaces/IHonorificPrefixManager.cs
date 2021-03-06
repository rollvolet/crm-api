using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IHonorificPrefixManager
    {
        Task<IEnumerable<HonorificPrefix>> GetAllAsync();
        Task<HonorificPrefix> GetByCustomerIdAsync(int id);
        Task<HonorificPrefix> GetByContactIdAsync(int id);
        Task<HonorificPrefix> GetByBuildingIdAsync(int id);
    }
}