using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IOrderDataProvider
    {
        Task<Paged<Order>> GetAllAsync(QuerySet query);
        Task<Order> GetByIdAsync(int id, QuerySet query);
        Task<Paged<Order>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Order> GetByOfferIdAsync(int offerId, QuerySet query = null);
    }
}