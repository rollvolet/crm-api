using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IVisitManager
    {
        Task<Visit> GetByIdAsync(int id, QuerySet query = null);
        Task<Visit> GetByRequestIdAsync(int id);
        Task<Visit> CreateAsync(Visit visit);
        Task<Visit> UpdateAsync(Visit visit);
        Task DeleteAsync(int id);
    }
}