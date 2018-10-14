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
        Task<Order> GetByOfferIdAsync(int offerId, QuerySet query = null);
        Task<Order> GetByInvoiceIdAsync(int invoiceId, QuerySet query = null);
        Task<Order> GetByDepositInvoiceIdAsync(int depositInvoiceId, QuerySet query = null);
        Task<Order> CreateAsync(Order order);
        Task<Order> UpdateAsync(Order order);
        Task DeleteAsync(int id);
    }
}
