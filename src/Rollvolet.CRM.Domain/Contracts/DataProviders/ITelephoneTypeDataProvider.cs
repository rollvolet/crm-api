using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ITelephoneTypeDataProvider
    {
        Task<IEnumerable<TelephoneType>> GetAllAsync();
        Task<TelephoneType> GetByIdAsync(int id);
        Task<TelephoneType> GetByTelephoneIdAsync(string id);
    }
}