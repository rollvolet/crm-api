using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers
{
    public class CustomerManager : ICustomerManager
    {
        private readonly ICustomerDataProvider _customerDataProvider;

        public CustomerManager(ICustomerDataProvider customerDataProvider)
        {
            _customerDataProvider = customerDataProvider;
        }
        
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _customerDataProvider.GetAll();
        }
    }
}