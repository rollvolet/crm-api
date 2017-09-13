using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ICustomerDataProvider
    {
        Task<Paged<Customer>> GetAllAsync(QuerySet query);
        Task<Customer> GetByIdAsync(int id, QuerySet query);
        Task<Customer> Create(Customer customer);
    }
}