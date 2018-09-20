using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IRequestDataProvider
    {
        Task<Paged<Request>> GetAllAsync(QuerySet query);
        Task<Request> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<Request>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Request> GetByOfferIdAsync(int offerId);
        Task<Request> GetByOrderIdAsync(int orderId);
        Task<Request> CreateAsync(Request request);
        Task<Request> UpdateAsync(Request request);
        Task DeleteByIdAsync(int id);
    }
}