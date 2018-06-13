using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Contracts.MsGraph
{
    public interface IGraphApiService
    {
        Task<Visit> CreateCalendarEventForVisit(Visit visit, CustomerEntity customerEntity);
        Task<Visit> UpdateCalendarEventForVisit(Visit visit, CustomerEntity customerEntity);
        Task<Visit> DeleteCalendarEventForVisit(Visit visit);
        Task<string> GetSubject(string msObjectId);
    }
}