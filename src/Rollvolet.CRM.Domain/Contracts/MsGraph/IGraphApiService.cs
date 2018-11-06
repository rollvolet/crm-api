using System;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Contracts.MsGraph
{
    public interface IGraphApiService
    {
        Task<Visit> CreateCalendarEventForVisitAsync(Visit visit, CustomerEntity customerEntity);
        Task<Visit> UpdateCalendarEventForVisitAsync(Visit visit, CustomerEntity customerEntity);
        Task<Visit> DeleteCalendarEventForVisitAsync(Visit visit);
        Task<Order> CreateCalendarEventForPlanningAsync(Order order);
        Task<Order> UpdateCalendarEventForPlanningAsync(Order order);
        Task<Order> DeleteCalendarEventForPlanningAsync(Order order);
        Task<string> GetVisitSubjectAsync(string msObjectId);
        Task<string> GetPlanningSubjectAsync(string msObjectId);
    }
}