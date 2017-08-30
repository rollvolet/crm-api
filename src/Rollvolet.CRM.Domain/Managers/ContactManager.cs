using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers
{
    public class ContactManager : IContactManager
    {
        private readonly IContactDataProvider _contactDataProvider;

        public ContactManager(IContactDataProvider contactDataProvider)
        {
            _contactDataProvider = contactDataProvider;
        }
        
        public async Task<Paged<Contact>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            return await _contactDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }
    }
}