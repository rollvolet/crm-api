using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface ICustomerManager
    {
        Task<Paged<Customer>> GetAllAsync(QuerySet query);
        Task<Customer> GetByIdAsync(int id, QuerySet query);
        Task<Customer> GetByRequestIdAsync(int requestId);
        Task<Customer> GetByInterventionIdAsync(int interventionId);
        Task<Customer> GetByOfferIdAsync(int offerId);
        Task<Customer> GetByOrderIdAsync(int orderId);
        Task<Customer> GetByInvoiceIdAsync(int invoiceId);
        Task<Customer> GetByDepositInvoiceIdAsync(int depositInvoiceId);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
    }
}
