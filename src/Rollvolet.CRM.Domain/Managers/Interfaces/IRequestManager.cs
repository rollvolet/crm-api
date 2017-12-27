using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IRequestManager
    {
        Task<Paged<Request>> GetAllAsync(QuerySet query);
        Task<Request> GetByIdAsync(int id, QuerySet query);
    }  
}
