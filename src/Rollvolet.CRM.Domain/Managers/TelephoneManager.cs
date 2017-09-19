using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class TelephoneManager : ITelephoneManager
    {
        private readonly ITelephoneDataProvider _telephoneDataProvider;

        public TelephoneManager(ITelephoneDataProvider telephoneDataProvider)
        {
            _telephoneDataProvider = telephoneDataProvider;
        }
        
        public async Task<Paged<Telephone>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            return await _telephoneDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }
    }
}