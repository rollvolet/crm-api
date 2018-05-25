using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IBuildingManager
    {
        Task<Paged<Building>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Building> GetByIdAsync(int id, QuerySet query);
        Task<Building> GetByTelephoneIdAsync(string telephoneId);
        Task<Building> GetByRequestIdAsync(int requestId);
        Task<Building> GetByOfferIdAsync(int offerId);
        Task<Building> CreateAsync(Building building);
        Task<Building> UpdateAsync(Building building);
        Task DeleteAsync(int id);
    }
}
