using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ICaseDataProvider
    {
        Task<Case> GetCaseByRequestIdAsync(int requestId);
        Task<Case> GetCaseByOfferIdAsync(int offerId);
        Task<Case> GetCaseByOrderIdAsync(int orderId);
        Task<Case> GetCaseByInvoiceIdAsync(int invoice);
    }
}