using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IVatRateDataProvider
    {
        Task<IEnumerable<VatRate>> GetAll();
        Task<VatRate> GetByIdAsync(int id);
    }
}