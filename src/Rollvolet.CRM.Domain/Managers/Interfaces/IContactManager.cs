using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IContactManager
    {
        Task<Paged<Contact>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
    }  
}
