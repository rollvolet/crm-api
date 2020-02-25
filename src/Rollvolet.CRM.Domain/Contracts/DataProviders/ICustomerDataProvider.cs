using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ICustomerDataProvider
    {
        Task<Paged<Customer>> GetAllAsync(QuerySet query);
        Task<Customer> GetByNumberAsync(int number, QuerySet query = null);
        Task<Customer> GetByTelephoneIdAsync(string telephoneId);
        Task<Customer> GetByRequestIdAsync(int requestId);
        Task<Customer> GetByInterventionIdAsync(int interventionId);
        Task<Customer> GetByOfferIdAsync(int offerId);
        Task<Customer> GetByOrderIdAsync(int orderId);
        Task<Customer> GetByInvoiceIdAsync(int invoiceId);
        Task<Customer> GetByDepositInvoiceIdAsync(int depositInvoiceId);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task DeleteByNumberAsync(int number);
    }
}