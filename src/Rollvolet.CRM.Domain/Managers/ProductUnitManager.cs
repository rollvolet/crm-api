using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers
{
    public class ProductUnitManager : IProductUnitManager
    {
        private readonly IProductUnitDataProvider _productUnitDataProvider;
        private readonly ILogger _logger;

        public ProductUnitManager(IProductUnitDataProvider productUnitDataProvider, ILogger<ProductUnitManager> logger)
        {
            _productUnitDataProvider = productUnitDataProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductUnit>> GetAllAsync()
        {
            return await _productUnitDataProvider.GetAllAsync();
        }

        public async Task<ProductUnit> GetByInvoiceSupplementIdAsync(int invoiceSupplementId)
        {
            return await _productUnitDataProvider.GetByInvoiceSupplementIdAsync(invoiceSupplementId);
        }
    }
}