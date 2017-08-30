using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IContactDataProvider
    {
        Task<Paged<Contact>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
    }
}