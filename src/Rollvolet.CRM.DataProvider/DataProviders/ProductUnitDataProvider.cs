using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.DataProviders
{
    public class ProductUnitDataProvider : IProductUnitDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ProductUnitDataProvider(CrmContext context, IMapper mapper, ILogger<ProductUnitDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductUnit>> GetAllAsync()
        {
            var productUnits = await Task.Run(() => _context.ProductUnits.OrderBy(c => c.Id).AsEnumerable());

            return _mapper.Map<IEnumerable<ProductUnit>>(productUnits);
        }

        public async Task<ProductUnit> GetByIdAsync(int id)
        {
            var unit = await _context.ProductUnits.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (unit == null)
            {
                _logger.LogError($"No product-unit found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<ProductUnit>(unit);
        }

        public async Task<ProductUnit> GetByInvoiceSupplementIdAsync(int invoiceSupplementId)
        {
            var productUnit = await _context.InvoiceSupplements.Where(c => c.Id == invoiceSupplementId).Select(c => c.Unit).FirstOrDefaultAsync();

            if (productUnit == null)
            {
                _logger.LogError($"No product-unit found for invoice-supplement with id {invoiceSupplementId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<ProductUnit>(productUnit);
        }
    }
}