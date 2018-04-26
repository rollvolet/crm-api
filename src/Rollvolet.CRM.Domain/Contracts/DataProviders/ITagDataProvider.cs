using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ITagDataProvider
    {
        Task<Paged<Tag>> GetAllByCustomerNumberAsync(int customerId, QuerySet query);
        Task<Tag> GetByIdAsync(int id);
    }
}