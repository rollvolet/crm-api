using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface ICustomerManager
    {
        Task<IEnumerable<Customer>> GetAllAsync();
    }  
}
