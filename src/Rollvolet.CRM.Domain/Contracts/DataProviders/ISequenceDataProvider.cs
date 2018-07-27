using System;
using System.Threading.Tasks;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ISequenceDataProvider
    {
        Task<int> GetNextCustomerNumber();
        Task<int> GetNextRelativeContactNumber(int customerId);
        Task<int> GetNextRelativeBuildingNumber(int customerId);
        Task<short> GetNextOfferSequenceNumber(DateTime date);
        Task<short> GetNextDepositSequenceNumber(int orderId);
    }
}