using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IEmployeeDataProvider
    {
        Task<IEnumerable<Employee>> GetAll();
        Task<Employee> GetByIdAsync(int id);
        Task<Employee> GetByFirstNameAsync(string name);
        Task<Employee> GetVisitorByOfferIdAsync(int offerId);
        Task<Employee> GetVisitorByOrderIdAsync(int offerId);
        Task<Employee> GetByWorkingHourIdAsync(int orderId);
    }
}