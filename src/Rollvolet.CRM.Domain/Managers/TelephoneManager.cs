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
            if (query.Sort.Field == null)
                query.Sort.Field = "order";

            return await _telephoneDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }
        
        public async Task<Paged<Telephone>> GetAllByContactIdAsync(int contactId, QuerySet query)
        {
            if (query.Sort.Field == null)
                query.Sort.Field = "order";

            return await _telephoneDataProvider.GetAllByContactIdAsync(contactId, query);
        }
        
        public async Task<Paged<Telephone>> GetAllByBuildingIdAsync(int buildingId, QuerySet query)
        {
            if (query.Sort.Field == null)
                query.Sort.Field = "order";

            return await _telephoneDataProvider.GetAllByBuildingIdAsync(buildingId, query);
        }
    }
}