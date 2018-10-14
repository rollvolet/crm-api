using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IBuildingDataProvider
    {
        Task<Building> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<Building>> GetAllByCustomerIdAsync(int customerId, QuerySet query);
        Task<Building> GetByTelephoneIdAsync(string telephoneId);
        Task<Building> GetByRequestIdAsync(int requestId);
        Task<Building> GetByOfferIdAsync(int offerId);
        Task<Building> GetByOrderIdAsync(int orderId);
        Task<Building> GetByInvoiceIdAsync(int invoiceId);
        Task<Building> GetByDepositInvoiceIdAsync(int invoiceId);
        Task<Building> CreateAsync(Building building);
        Task<Building> UpdateAsync(Building building);
        Task DeleteByIdAsync(int id);
    }
}