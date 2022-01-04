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
using Rollvolet.CRM.Domain.Models.Query;

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

        public async Task<IEnumerable<VatRate>> GetAllAsync()
        {
            var vatRates = await _context.VatRates.OrderBy(c => c.Order).ToListAsync();

            return _mapper.Map<IEnumerable<VatRate>>(vatRates);
        }

        public async Task<VatRate> GetByIdAsync(int id, QuerySet query = null)
        {
            var vatRate = await _context.VatRates.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (vatRate == null)
            {
                _logger.LogError($"No vat-rate found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<VatRate>(vatRate);
        }

        public async Task<VatRate> GetByOfferIdAsync(int offerId)
        {
            var vatRate = await _context.Offers.Where(c => c.Id == offerId).Select(c => c.VatRate).FirstOrDefaultAsync();

            if (vatRate == null)
            {
                _logger.LogError($"No vat-rate found for offer with id {offerId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<VatRate>(vatRate);
        }

        public async Task<VatRate> GetByOrderIdAsync(int orderId)
        {
            var vatRate = await _context.Orders.Where(c => c.Id == orderId).Select(c => c.VatRate).FirstOrDefaultAsync();

            if (vatRate == null)
            {
                _logger.LogError($"No vat-rate found for order with id {orderId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<VatRate>(vatRate);
        }

        public async Task<VatRate> GetByInvoiceIdAsync(int invoiceId)
        {
            var vatRate = await _context.Invoices.Where(c => c.Id == invoiceId).Select(c => c.VatRate).FirstOrDefaultAsync();

            if (vatRate == null)
            {
                _logger.LogError($"No vat-rate found for invoice with id {invoiceId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<VatRate>(vatRate);
        }

        public async Task<VatRate> GetByDepositInvoiceIdAsync(int depositInvoiceId)
        {
            return await GetByInvoiceIdAsync(depositInvoiceId);
        }
    }
}