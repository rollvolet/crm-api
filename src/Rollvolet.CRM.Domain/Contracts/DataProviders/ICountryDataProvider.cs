using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ICountryDataProvider
    {
        Task<IEnumerable<Country>> GetAll();
        Task<Country> GetByIdAsync(int id);
        Task<Country> GetByContactIdAsync(int id);
        Task<Country> GetByBuildingIdAsync(int id);
        Task<Country> GetByTelephoneIdAsync(string id);
    }
}