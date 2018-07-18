using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IOfferManager
    {
        Task<Paged<Offer>> GetAllAsync(QuerySet query);
        Task<Offer> GetByIdAsync(int id, QuerySet query);
        Task<Paged<Offer>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Offer> GetByRequestIdAsync(int requestId, QuerySet query = null);
        Task<Offer> CreateAsync(Offer offer);
        Task<Offer> UpdateAsync(Offer offer);
        Task DeleteAsync(int id);
    }
}
