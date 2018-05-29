using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IVisitManager
    {
        Task<Visit> GetByRequestIdAsync(int id);
    }
}