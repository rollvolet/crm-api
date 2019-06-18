using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IAccountancyExportDataProvider
    {
        Task<Paged<AccountancyExport>> GetAllAsync(QuerySet query);
        Task<AccountancyExport> GetByIdAsync(int id, QuerySet query = null);
        Task<AccountancyExport> CreateAsync(AccountancyExport accountancyExport);
    }
}