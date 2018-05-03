using System.Threading.Tasks;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ISequenceDataProvider
    {
        Task<int> GetNextCustomerNumber();
        Task<int> GetNextRelativeCustomerNumber(int customerId);
    }
}