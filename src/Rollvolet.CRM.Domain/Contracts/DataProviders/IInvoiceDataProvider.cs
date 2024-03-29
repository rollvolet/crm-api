using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IInvoiceDataProvider
    {
        Task<Paged<Invoice>> GetAllAsync(QuerySet query);
        Task<Invoice> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<Invoice>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Paged<Invoice>> GetAllByRelativeContactIdAsync(int customerId, int relativeContactId, QuerySet query);
        Task<Paged<Invoice>> GetAllByRelativeBuildingIdAsync(int customerId, int relativeBuildingId, QuerySet query);
        Task<Invoice> GetByOrderIdAsync(int orderId, QuerySet query = null);
        Task<Invoice> GetByInterventionIdAsync(int interventionId, QuerySet query = null);
        Task<Invoice> CreateAsync(Invoice invoice);
        Task<Invoice> UpdateAsync(Invoice invoice);
        Task<Invoice> UpdateContactAndBuildingAsync(int id, int? relativeContactId, int? relativeBuildingId);
        Task<Invoice> UpdateCachedInvoiceAmountsAsync(int id);
        Task DeleteByIdAsync(int id);
    }
}