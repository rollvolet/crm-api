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
using System.Text.RegularExpressions;

namespace Rollvolet.CRM.DataProviders
{   
    public class TelephoneDataProvider : ITelephoneDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private static Regex _digitsOnly = new Regex(@"[^\d]");  

        public TelephoneDataProvider(CrmContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Paged<Telephone>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Number == customerId);

            // TODO throw EntityNotFound if customer is null
            return await this.GetAllByCustomerDataIdAsync(customer.DataId, query);
        }

        public async Task<Paged<Telephone>> GetAllByContactIdAsync(int contactId, QuerySet query)
        {
            return await this.GetAllByCustomerDataIdAsync(contactId, query);
        }

        public async Task<Paged<Telephone>> GetAllByBuildingIdAsync(int contactId, QuerySet query)
        {
            return await this.GetAllByCustomerDataIdAsync(contactId, query);
        }

        private async Task<Paged<Telephone>> GetAllByCustomerDataIdAsync(int dataId, QuerySet query)
        {
            var source = _context.Telephones
                            .Where(t => t.CustomerRecordId == dataId)
                            .Include(query)
                            .Sort(query);

            var telephones = source.Skip(query.Page.Skip).Take(query.Page.Take).AsEnumerable();

            var count = await source.CountAsync();

            var mappedTelephones = _mapper.Map<IEnumerable<Telephone>>(telephones);

            return new Paged<Telephone>() {
                Items = mappedTelephones,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }
    }
}