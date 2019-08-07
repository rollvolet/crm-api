using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IWayOfEntryDataProvider
    {
        Task<IEnumerable<WayOfEntry>> GetAllAsync();
        Task<WayOfEntry> GetByIdAsync(int id);
        Task<WayOfEntry> GetByRequestIdAsync(int id);
    }
}