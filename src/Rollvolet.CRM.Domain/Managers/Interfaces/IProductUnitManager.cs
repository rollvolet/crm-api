using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IProductUnitManager
    {
        Task<IEnumerable<ProductUnit>> GetAllAsync();
        Task<ProductUnit> GetByInvoiceSupplementIdAsync(int invoiceSupplementId);
    }
}