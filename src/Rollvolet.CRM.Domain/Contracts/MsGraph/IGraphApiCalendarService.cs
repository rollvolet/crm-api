using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Contracts.MsGraph
{
    public interface IGraphApiCalendarService
    {
        Task<Order> CreateEventForPlanningAsync(Order order);
        Task<Order> UpdateEventForPlanningAsync(Order order, bool requiresReschedule);
        Task<Order> DeleteEventForPlanningAsync(Order order);
        Task <PlanningEvent> EnrichPlanningEventAsync(PlanningEvent planningEvent);
        Task<PlanningEvent> CreateEventForPlanningAsync(PlanningEvent planningEvent);
        Task<PlanningEvent> UpdateEventForPlanningAsync(PlanningEvent planningEvent, bool requiresReschedule);
        Task<PlanningEvent> DeleteEventForPlanningAsync(PlanningEvent planningEvent);
        Task<string> GetPlanningSubjectAsync(string msObjectId);
    }
}