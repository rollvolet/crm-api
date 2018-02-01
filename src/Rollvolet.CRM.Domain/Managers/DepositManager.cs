using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class DepositManager : IDepositManager
    {
        private readonly IDepositDataProvider _depositDataProvider;

        public DepositManager(IDepositDataProvider depositDataProvider)
        {
            _depositDataProvider = depositDataProvider;
        }
        
        public async Task<Paged<Deposit>> GetAllByOrderIdAsync(int orderId, QuerySet query)
        {
            if (query.Sort.Field == null) {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "payment-date";
            }

            return await _depositDataProvider.GetAllByOrderIdAsync(orderId, query);
        }
    }
}