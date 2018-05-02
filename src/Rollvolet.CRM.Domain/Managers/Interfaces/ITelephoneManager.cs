using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface ITelephoneManager
    {
        Task<Paged<Telephone>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Paged<Telephone>> GetAllByContactIdAsync(int contactId, QuerySet query);
        Task<Paged<Telephone>> GetAllByBuildingIdAsync(int contactId, QuerySet query); 
        Task<Telephone> CreateAsync(Telephone telephone);
        Task DeleteAsync(string composedId);
    }  
}
