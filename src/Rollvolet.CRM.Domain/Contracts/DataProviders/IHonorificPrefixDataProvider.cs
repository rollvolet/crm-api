using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IHonorificPrefixDataProvider
    {
        Task<IEnumerable<HonorificPrefix>> GetAllAsync();
        Task<HonorificPrefix> GetByIdAsync(string id);
        Task<HonorificPrefix> GetByCustomerNumberAsync(int number);
        Task<HonorificPrefix> GetByContactIdAsync(int id);
        Task<HonorificPrefix> GetByBuildingIdAsync(int id);
    }
}