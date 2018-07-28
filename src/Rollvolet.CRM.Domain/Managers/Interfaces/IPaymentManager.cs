using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IPaymentManager
    {
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Payment> GetByDepositIdAsync(int depositId);
    }
}