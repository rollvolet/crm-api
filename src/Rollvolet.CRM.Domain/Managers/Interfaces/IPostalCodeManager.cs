using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IPostalCodeManager
    {
        Task<IEnumerable<PostalCode>> GetAllAsync(); 
    }
}