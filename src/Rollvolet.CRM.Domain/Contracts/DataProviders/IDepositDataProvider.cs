using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IDepositDataProvider
    {
        Task<Deposit> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<Deposit>> GetAllByOrderIdAsync(int orderId, QuerySet query);
        Task<Deposit> CreateAsync(Deposit deposit);
        Task<Deposit> UpdateAsync(Deposit deposit);
        Task DeleteByIdAsync(int id);
    }
}