using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ICaseDataProvider
    {
        Task<Case> GetCaseByRequestIdAsync(int requestId);
        Task<Case> GetCaseByInterventionIdAsync(int interentionId);
        Task<Case> GetCaseByOfferIdAsync(int offerId);
        Task<Case> GetCaseByOrderIdAsync(int orderId);
        Task<Case> GetCaseByInvoiceIdAsync(int invoice);
    }
}