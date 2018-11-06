using System;
using System.Threading.Tasks;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ISequenceDataProvider
    {
        Task<int> GetNextCustomerNumberAsync();
        Task<int> GetNextInvoiceNumberAsync();
        Task<int> GetNextRelativeContactNumberAsync(int customerId);
        Task<int> GetNextRelativeBuildingNumberAsync(int customerId);
        Task<short> GetNextOfferSequenceNumberAsync(DateTime date);
        Task<short> GetNextDepositSequenceNumberAsync(int orderId);
    }
}