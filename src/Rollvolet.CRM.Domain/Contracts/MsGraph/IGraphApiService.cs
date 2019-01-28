using System;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Contracts.MsGraph
{
    public interface IGraphApiService
    {
        Task<CalendarEvent> CreateEventForRequestAsync(CalendarEvent calendarEvent, CustomerEntity customerEntity);
        Task<CalendarEvent> UpdateEventForRequestAsync(CalendarEvent calendarEvent, CustomerEntity customerEntity);
        Task<CalendarEvent> DeleteEventForRequestAsync(CalendarEvent calendarEvent);
        Task<Order> CreateEventForPlanningAsync(Order order);
        Task<Order> UpdateEventForPlanningAsync(Order order);
        Task<Order> DeleteEventForPlanningAsync(Order order);
        Task<string> GetVisitSubjectAsync(string msObjectId);
        Task<string> GetPlanningSubjectAsync(string msObjectId);
    }
}