using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IDepositInvoiceDataProvider
    {
        Task<Paged<DepositInvoice>> GetAllAsync(QuerySet query);
        Task<DepositInvoice> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<DepositInvoice>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Paged<DepositInvoice>> GetAllByOrderIdAsync(int orderId, QuerySet query);
        Task<Paged<DepositInvoice>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query);
        Task<DepositInvoice> CreateAsync(DepositInvoice depositInvoice);
        Task<DepositInvoice> UpdateAsync(DepositInvoice depositInvoice);
        Task<DepositInvoice> UpdateContactAndBuildingAsync(int id, int? relativeContactId, int? relativeBuildingId);
        Task DeleteByIdAsync(int id);
    }
}