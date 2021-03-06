using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IVatRateDataProvider
    {
        Task<IEnumerable<VatRate>> GetAllAsync();
        Task<VatRate> GetByIdAsync(int id, QuerySet query = null);
        Task<VatRate> GetByOfferIdAsync(int offerId);
        Task<VatRate> GetByOrderIdAsync(int orderId);
        Task<VatRate> GetByOfferlineIdAsync(int offerlineId);
        Task<VatRate> GetByInvoicelineIdAsync(int invoicelineId);
        Task<VatRate> GetByInvoiceIdAsync(int invoiceId);
        Task<VatRate> GetByDepositInvoiceIdAsync(int invoiceId);
    }
}