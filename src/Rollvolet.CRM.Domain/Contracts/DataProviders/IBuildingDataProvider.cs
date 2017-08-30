using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IBuildingDataProvider
    {
        Task<Paged<Building>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
    }
}