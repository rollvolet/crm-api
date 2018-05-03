using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IBuildingDataProvider
    {
        Task<Building> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<Building>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Building> CreateAsync(Building building);
        Task DeleteByIdAsync(int id);
    }
}