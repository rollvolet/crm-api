using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IOfferlineDataProvider
    {
        Task<Offerline> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<Offerline>> GetAllByOfferIdAsync(int offerId, QuerySet query);
        Task<Offerline> CreateAsync(Offerline offerline);
        Task<Offerline> UpdateAsync(Offerline offerline);
        Task DeleteByIdAsync(int id);
    }
}