using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IPlanningEventManager
    {
        Task<PlanningEvent> GetByIdAsync(int id, QuerySet query);
        Task<PlanningEvent> UpdateAsync(PlanningEvent planningEvent);
    }
}
