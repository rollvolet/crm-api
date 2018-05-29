using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IVisitDataProvider
    {
        Task<Visit> GetByIdAsync(int id, QuerySet query = null);
        Task<Visit> GetByRequestIdAsync(int id);
        Task<Visit> CreateAsync(Visit visit);
        Task<Visit> UpdateAsync(Visit visit);
        Task DeleteByIdAsync(int visit);
    }
}