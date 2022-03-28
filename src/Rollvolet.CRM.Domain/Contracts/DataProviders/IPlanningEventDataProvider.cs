using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IPlanningEventDataProvider
    {
        Task<PlanningEvent> GetByIdAsync(int id, QuerySet query = null);
        Task<PlanningEvent> UpdateAsync(PlanningEvent planningEvent);
    }
}