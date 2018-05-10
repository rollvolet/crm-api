using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface ILanguageManager
    {
        Task<IEnumerable<Language>> GetAllAsync();
        Task<Language> GetByContactIdAsync(int id);
        Task<Language> GetByBuildingIdAsync(int id);
    }
}