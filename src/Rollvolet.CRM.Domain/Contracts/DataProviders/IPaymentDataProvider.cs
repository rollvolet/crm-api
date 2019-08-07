using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IPaymentDataProvider
    {
        Task<Payment> GetByIdAsync(string id);
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Payment> GetByDepositIdAsync(int depositId);
    }
}