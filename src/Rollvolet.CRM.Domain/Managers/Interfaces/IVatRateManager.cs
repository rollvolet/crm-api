using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IVatRateManager
    {
        Task<IEnumerable<VatRate>> GetAllAsync();
    }
}