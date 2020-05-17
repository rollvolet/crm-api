using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IDepositInvoiceManager
    {
        Task<Paged<DepositInvoice>> GetAllAsync(QuerySet query);
        Task<DepositInvoice> GetByIdAsync(int id, QuerySet query);
        Task<Paged<DepositInvoice>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Paged<DepositInvoice>> GetAllByOrderIdAsync(int orderId, QuerySet query);
        Task<Paged<DepositInvoice>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query);
        Task<DepositInvoice> CreateAsync(DepositInvoice depositInvoice);
        Task<DepositInvoice> UpdateAsync(DepositInvoice depositInvoice);
        Task DeleteAsync(int id);
    }
}
