using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IWayOfEntryManager
    {
        Task<IEnumerable<WayOfEntry>> GetAllAsync();
        Task<WayOfEntry> GetByRequestIdAsync(int id);
        Task<WayOfEntry> GetByInterventionIdAsync(int id);
    }
}