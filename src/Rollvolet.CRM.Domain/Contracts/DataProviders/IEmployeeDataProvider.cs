using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IEmployeeDataProvider
    {
        Task<IEnumerable<Employee>> GetAll();
        Task<Employee> GetByFirstName(string name);
        Task<Employee> GetVisitorByOfferId(int offerId);
        Task<Employee> GetVisitorByOrderId(int offerId);
    }
}