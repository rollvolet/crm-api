using System.Threading.Tasks;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ISystemTaskDataProvider
    {
        Task RecalcalulateSearchNames();
        Task RenameOfferDocuments(string[] locations);
    }
}