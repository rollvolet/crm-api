using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ILanguageDataProvider
    {
        Task<IEnumerable<Language>> GetAllAsync();
        Task<Language> GetByIdAsync(int id);
        Task<Language> GetByCustomerNumberAsync(int number);
        Task<Language> GetByContactIdAsync(int id);
        Task<Language> GetByBuildingIdAsync(int id);
    }
}