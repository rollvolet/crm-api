using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface ICustomerManager
    {
        Task<Paged<Customer>> GetAllAsync(QuerySet query);
        Task<Customer> GetByIdAsync(int id, QuerySet query);
        Task<Customer> GetByTelephoneIdAsync(string telephoneId);
        Task<Customer> GetByRequestIdAsync(int requestId);
        Task<Customer> GetByOfferIdAsync(int offerId);
        Task<Customer> GetByOrderIdAsync(int orderId);
        Task<Customer> GetByInvoiceIdAsync(int invoiceId);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
    }
}
