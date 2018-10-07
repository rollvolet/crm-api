using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IDepositManager
    {
        Task<Paged<Deposit>> GetAllByOrderIdAsync(int orderId, QuerySet query);
        Task<Paged<Deposit>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query);
        Task<Deposit> CreateAsync(Deposit deposit);
        Task<Deposit> UpdateAsync(Deposit deposit);
        Task DeleteAsync(int id);
    }
}
