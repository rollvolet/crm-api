using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IDepositDataProvider
    {
        Task<Paged<Deposit>> GetAllByOrderIdAsync(int customerId, QuerySet query);
    }
}