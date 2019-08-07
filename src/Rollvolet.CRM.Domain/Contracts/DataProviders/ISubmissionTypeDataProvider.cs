using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface ISubmissionTypeDataProvider
    {
        Task<IEnumerable<SubmissionType>> GetAllAsync();
        Task<SubmissionType> GetByIdAsync(string id);
        Task<SubmissionType> GetByOfferIdAsync(int id);
    }
}