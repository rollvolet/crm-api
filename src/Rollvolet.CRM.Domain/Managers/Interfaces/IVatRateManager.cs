using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IVatRateManager
    {
        Task<IEnumerable<VatRate>> GetAllAsync();
        Task<VatRate> GetByIdAsync(int id, QuerySet query = null);
        Task<VatRate> GetByOfferIdAsync(int offerId);
        Task<VatRate> GetByOrderIdAsync(int orderId);
        Task<VatRate> GetByInvoiceIdAsync(int invoiceId);
        Task<VatRate> GetByDepositInvoiceIdAsync(int depositInvoiceId);
    }
}