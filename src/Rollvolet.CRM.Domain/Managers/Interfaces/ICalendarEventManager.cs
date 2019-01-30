using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface ICalendarEventManager
    {
        Task<CalendarEvent> GetByIdAsync(int id, QuerySet query = null);
        Task<CalendarEvent> GetByRequestIdAsync(int id);
        Task<CalendarEvent> CreateAsync(CalendarEvent calendarEvent);
        Task<CalendarEvent> UpdateAsync(CalendarEvent calendarEvent, bool forceEventUpdate = false);
        Task DeleteAsync(int id);
    }
}