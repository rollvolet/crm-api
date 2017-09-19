using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ITelephoneDataProvider
    {
        Task<Paged<Telephone>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
    }
}