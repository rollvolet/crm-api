using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IEmployeeDataProvider
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee> GetByIdAsync(int id);
        Task<Employee> GetByFirstNameAsync(string name);
        Task<Employee> GetVisitorByOfferIdAsync(int offerId);
        Task<Employee> GetVisitorByOrderIdAsync(int offerId);
        Task<Employee> GetByInterventionIdAsync(int interventionId);
        Task<Paged<Employee>> GetAllByInterventionIdAsync(int interventionId, QuerySet query);
        Task<Paged<Employee>> GetAllByOrderIdAsync(int orderId, QuerySet query);
    }
}