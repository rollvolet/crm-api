using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ILanguageDataProvider
    {
        Task<IEnumerable<Language>> GetAll();
        Task<Language> GetByIdAsync(int id);
    }
}