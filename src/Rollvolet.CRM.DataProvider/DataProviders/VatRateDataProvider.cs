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
    public class VatRateDataProvider : IVatRateDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public VatRateDataProvider(CrmContext context, IMapper mapper, ILogger<HonorificPrefixDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<VatRate>> GetAll()
        {
            var vatRates = _context.VatRates.OrderBy(c => c.Order).AsEnumerable();

            return _mapper.Map<IEnumerable<VatRate>>(vatRates);
        }

        public async Task<VatRate> GetByIdAsync(int id)
        {
            var vatRate = await _context.VatRates.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (vatRate == null)
            {
                _logger.LogError($"No vat-rate found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<VatRate>(vatRate);
        }
    }
}