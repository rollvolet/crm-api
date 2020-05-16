using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.DataProvider.Extensions;
using Rollvolet.CRM.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Rollvolet.CRM.DataProviders
{
    public class DepositDataProvider : IDepositDataProvider
    {
        private readonly ISequenceDataProvider _sequenceDataProvider;
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DepositDataProvider(ISequenceDataProvider sequenceDataProvider, CrmContext context, IMapper mapper, ILogger<DepositDataProvider> logger)
        {
            _sequenceDataProvider = sequenceDataProvider;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Deposit> GetByIdAsync(int id, QuerySet query = null)
        {
            var deposit = await FindByIdAsync(id, query);

            if (deposit == null)
            {
                _logger.LogError($"No deposit found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Deposit>(deposit);
        }

        public async Task<Paged<Deposit>> GetAllByOrderIdAsync(int orderId, QuerySet query)
        {
            var source = _context.Deposits
                            .Where(c => c.OrderId == orderId)
                            .Include(query)
                            .Sort(query)
                            .Filter(query);

            var deposits = await source.ForPage(query).ToListAsync();

            var count = await source.CountAsync();

            var mappedDeposits = _mapper.Map<IEnumerable<Deposit>>(deposits);

            return new Paged<Deposit>() {
                Items = mappedDeposits,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Paged<Deposit>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query)
        {
            var source = _context.Deposits
                            .Where(c => c.InvoiceId == invoiceId)
                            .Include(query)
                            .Sort(query)
                            .Filter(query);

            var deposits = await source.ForPage(query).ToListAsync();

            var count = await source.CountAsync();

            var mappedDeposits = _mapper.Map<IEnumerable<Deposit>>(deposits);

            return new Paged<Deposit>() {
                Items = mappedDeposits,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Deposit> CreateAsync(Deposit deposit)
        {
            var depositRecord = _mapper.Map<DataProvider.Models.Deposit>(deposit);

            if (depositRecord.OrderId != null)
            {
                depositRecord.SequenceNumber = await _sequenceDataProvider.GetNextDepositSequenceNumberAsync((int) depositRecord.OrderId);

                var invoice = await _context.Orders.Where(o => o.Id == depositRecord.OrderId).Select(o => o.Invoice).FirstOrDefaultAsync();

                if (invoice != null)
                    depositRecord.InvoiceId = invoice.Id;
            }

            depositRecord.Currency = "EUR";
            depositRecord.IsDeposit = true; // should be set to false if created while an invoice is already attached?

            _context.Deposits.Add(depositRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Deposit>(depositRecord);
        }

        public async Task<Deposit> UpdateAsync(Deposit deposit)
        {
            var depositRecord = await FindByIdAsync(deposit.Id);
            _mapper.Map(deposit, depositRecord);

            depositRecord.Currency = "EUR";
            depositRecord.IsDeposit = true;

            _context.Deposits.Update(depositRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Deposit>(depositRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var deposit = await FindByIdAsync(id);

            if (deposit != null)
            {
                _context.Deposits.Remove(deposit);
                await _context.SaveChangesAsync();
           }
        }

        private async Task<DataProvider.Models.Deposit> FindByIdAsync(int id, QuerySet query = null)
        {
            return await FindWhereAsync(c => c.Id == id, query);
        }

        private async Task<DataProvider.Models.Deposit> FindWhereAsync(Expression<Func<DataProvider.Models.Deposit, bool>> where, QuerySet query = null)
        {
            var source = _context.Deposits.Where(where);

            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }
    }
}