using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IOfferDataProvider
    {
        Task<Paged<Offer>> GetAllAsync(QuerySet query);
        Task<Offer> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<Offer>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Offer> GetByOfferNumberAsync(string offerNumber, QuerySet query = null);
        Task<Offer> GetByRequestIdAsync(int requestId, QuerySet query = null);
        Task<Offer> GetByOrderIdAsync(int orderId, QuerySet query = null);
        Task<Offer> CreateAsync(Offer offer);
        Task<Offer> UpdateAsync(Offer offer);
        Task<Offer> UpdateContactAndBuildingAsync(int id, int? relativeContactId, int? relativeBuildingId);
        Task DeleteByIdAsync(int id);
    }
}