using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IOrderManager
    {
        Task<Paged<Order>> GetAllAsync(QuerySet query);
        Task<Order> GetByIdAsync(int id, QuerySet query);
        Task<Paged<Order>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
    }  
}
