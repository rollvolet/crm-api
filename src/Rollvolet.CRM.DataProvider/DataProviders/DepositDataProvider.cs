using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Narato.ResponseMiddleware.Models.Exceptions;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.DataProvider.Extensions;

namespace Rollvolet.CRM.DataProviders
{   
    public class DepositDataProvider : IDepositDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;

        public DepositDataProvider(CrmContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Paged<Deposit>> GetAllByOrderIdAsync(int orderId, QuerySet query)
        {
            var source = _context.Deposits
                            .Where(c => c.OrderId == orderId)
                            .Include(query)
                            .Sort(query)
                            .Filter(query);

            var deposits = source.ForPage(query).AsEnumerable();

            var count = await source.CountAsync();

            var mappedDeposits = _mapper.Map<IEnumerable<Deposit>>(deposits);

            return new Paged<Deposit>() {
                Items = mappedDeposits,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }
    }
}