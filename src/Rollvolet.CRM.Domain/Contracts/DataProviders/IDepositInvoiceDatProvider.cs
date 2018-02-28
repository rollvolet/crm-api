using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IDepositInvoiceDataProvider
    {
        Task<Paged<DepositInvoice>> GetAllByOrderIdAsync(int orderId, QuerySet query);
    }
}