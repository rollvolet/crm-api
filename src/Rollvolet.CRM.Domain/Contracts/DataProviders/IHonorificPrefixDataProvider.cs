using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IHonorificPrefixDataProvider
    {
        Task<IEnumerable<HonorificPrefix>> GetAll();
        Task<HonorificPrefix> GetByIdAsync(string id);
    }
}