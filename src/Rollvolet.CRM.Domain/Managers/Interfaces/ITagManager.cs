using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface ITagManager
    {
        Task<Paged<Tag>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
    }  
}
