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
using Microsoft.Extensions.Logging;

namespace Rollvolet.CRM.DataProviders
{   
    public class TelephoneDataProvider : ITelephoneDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private static Regex _digitsOnly = new Regex(@"[^\d]");  

        public TelephoneDataProvider(CrmContext context, IMapper mapper, ILogger<TelephoneDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Paged<Telephone>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Number == customerId);

            if (customer == null)
            {
                _logger.LogError($"No customer found with number {customerId}");
                throw new EntityNotFoundException();
            }
            
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

            var telephones = source.ForPage(query).AsEnumerable();

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