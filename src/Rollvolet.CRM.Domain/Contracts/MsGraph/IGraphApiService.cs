using System;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Contracts.MsGraph
{
    public interface IGraphApiService
    {
        Task<Visit> CreateCalendarEventForVisit(Visit visit, CustomerEntity customerEntity);
        Task<Visit> UpdateCalendarEventForVisit(Visit visit, CustomerEntity customerEntity);
        Task<Visit> DeleteCalendarEventForVisit(Visit visit);
        Task<Order> CreateCalendarEventForPlanning(Order order);
        Task<Order> UpdateCalendarEventForPlanning(Order order);
        Task<Order> DeleteCalendarEventForPlanning(Order order);
        Task<string> GetVisitSubject(string msObjectId);
        Task<string> GetPlanningSubject(string msObjectId);
    }
}