using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IProductUnitDataProvider
    {
        Task<IEnumerable<ProductUnit>> GetAllAsync();
        Task<ProductUnit> GetByIdAsync(int id);
        Task<ProductUnit> GetByInvoiceSupplementIdAsync(int id);
    }
}