using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IVatRateManager
    {
        Task<IEnumerable<VatRate>> GetAllAsync();
        Task<VatRate> GetByOfferIdAsync(int offerId);
        Task<VatRate> GetByOrderIdAsync(int orderId);
        Task<VatRate> GetByOfferlineIdAsync(int offerlineId);
        Task<VatRate> GetByInvoicelineIdAsync(int invoicelineId);
        Task<VatRate> GetByInvoiceIdAsync(int invoiceId);
        Task<VatRate> GetByDepositInvoiceIdAsync(int depositInvoiceId);
    }
}