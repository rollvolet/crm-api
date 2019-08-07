using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IPostalCodeDataProvider
    {
        Task<IEnumerable<PostalCode>> GetAllAsync();
        Task<PostalCode> GetByIdAsync(int id);
    }
}