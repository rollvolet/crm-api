using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Exceptions;

namespace Rollvolet.CRM.DataProviders
{
    public class PaymentDataProvider : IPaymentDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PaymentDataProvider(CrmContext context, IMapper mapper, ILogger<PaymentDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            var payments = await _context.Payments.OrderBy(c => c.Name).ToListAsync();

            return _mapper.Map<IEnumerable<Payment>>(payments);
        }

        public async Task<Payment> GetByIdAsync(string id)
        {
            var payment = await _context.Payments.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (payment == null)
            {
                _logger.LogError($"No payment found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Payment>(payment);
        }

        public async Task<Payment> GetByDepositIdAsync(int depositId)
        {
            var payment = await _context.Deposits.Where(c => c.Id == depositId).Select(c => c.Payment).FirstOrDefaultAsync();

            if (payment == null)
            {
                _logger.LogError($"No payment found for deposit with id {depositId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Payment>(payment);
        }
    }
}