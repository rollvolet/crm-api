using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IInvoiceManager
    {
        Task<Paged<Invoice>> GetAllAsync(QuerySet query);
        Task<Invoice> GetByIdAsync(int id, QuerySet query);
        Task<Paged<Invoice>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
    }  
}