using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IContactDataProvider
    {
        Task<Contact> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<Contact>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Contact> GetByTelephoneIdAsync(string telephoneId);
        Task<Contact> CreateAsync(Contact contact);
        Task<Contact> UpdateAsync(Contact contact);
        Task DeleteByIdAsync(int id);
    }
}