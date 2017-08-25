using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ICustomerDataProvider
    {
        Task<IEnumerable<Customer>> GetAll();
    }
}