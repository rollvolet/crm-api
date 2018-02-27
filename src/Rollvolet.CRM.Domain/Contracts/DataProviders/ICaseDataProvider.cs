using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ICaseDataProvider
    {
        Task<Case> GetCaseByRequestId(int requestId);
        Task<Case> GetCaseByOfferId(int offerId);
        Task<Case> GetCaseByOrderId(int orderId);
        Task<Case> GetCaseByInvoiceId(int invoice);      
    }
}