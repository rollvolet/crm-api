using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IEmployeeManager
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee> GetByInterventionIdAsync(int interventionId);
        Task<Paged<Employee>> GetAllByInterventionIdAsync(int interventionId, QuerySet query);
        Task<Paged<Employee>> GetAllByOrderIdAsync(int orderId, QuerySet query);
    }
}