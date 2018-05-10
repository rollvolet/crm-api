using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface ICountryManager
    {
        Task<IEnumerable<Country>> GetAllAsync();
        Task<Country> GetByContactIdAsync(int id);
        Task<Country> GetByBuildingIdAsync(int id);
        Task<Country> GetByTelephoneIdAsync(string id);
    }
}