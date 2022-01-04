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
        Task<Paged<Invoice>> GetAllByContactIdAsync(int contactId, QuerySet query);
        Task<Paged<Invoice>> GetAllByBuildingIdAsync(int contactId, QuerySet query);
        Task<Invoice> GetByOrderIdAsync(int orderId, QuerySet query = null);
        Task<Invoice> GetByInterventionIdAsync(int interventionId, QuerySet query = null);
        Task<Invoice> GetByWorkingHourIdAsync(int workingHourId);
        Task<Invoice> CreateAsync(Invoice invoice);
        Task<Invoice> UpdateAsync(Invoice invoice);
        Task DeleteAsync(int id);
    }
}
