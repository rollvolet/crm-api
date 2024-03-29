using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IContactDataProvider
    {
        Task<Contact> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<Contact>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Contact> GetByRequestIdAsync(int requestId);
        Task<Contact> GetByInterventionIdAsync(int interventionId);
        Task<Contact> GetByOfferIdAsync(int offerId);
        Task<Contact> GetByOrderIdAsync(int orderId);
        Task<Contact> GetByInvoiceIdAsync(int invoiceId);
        Task<Contact> GetByDepositInvoiceIdAsync(int invoiceId);
        Task<Contact> CreateAsync(Contact contact);
        Task<Contact> UpdateAsync(Contact contact);
        Task DeleteByIdAsync(int id);
    }
}