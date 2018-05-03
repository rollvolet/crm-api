using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IContactManager
    {
        Task<Paged<Contact>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Contact> GetByIdAsync(int id, QuerySet query);
        Task<Contact> CreateAsync(Contact contact);
        Task DeleteAsync(int id);
    }  
}
