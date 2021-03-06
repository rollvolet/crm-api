using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IVisitDataProvider
    {
        Task<CalendarEvent> GetByIdAsync(int id, QuerySet query = null);
        Task<CalendarEvent> GetByRequestIdAsync(int id);
        Task<CalendarEvent> UpdateAsync(CalendarEvent calendarEvent);
    }
}