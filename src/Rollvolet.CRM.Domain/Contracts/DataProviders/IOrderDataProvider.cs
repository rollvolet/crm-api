using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IOrderDataProvider
    {
        Task<Paged<Order>> GetAllAsync(QuerySet query);
        Task<Order> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<Order>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Order> GetByOfferIdAsync(int offerId, QuerySet query = null);
        Task<Order> GetByInvoiceIdAsync(int invoiceId, QuerySet query = null);
        Task<Order> GetByInvoicelineIdAsync(int invoicelineId);
        Task<Order> GetByDepositInvoiceIdAsync(int invoiceId, QuerySet query = null);
        Task<Order> CreateAsync(Order order);
        Task<Order> UpdateAsync(Order order);
        Task DeleteByIdAsync(int id);
    }
}