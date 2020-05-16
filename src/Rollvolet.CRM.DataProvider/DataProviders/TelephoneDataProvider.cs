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
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Exceptions;

namespace Rollvolet.CRM.DataProviders
{
    public class TelephoneDataProvider : ITelephoneDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public TelephoneDataProvider(CrmContext context, IMapper mapper, ILogger<TelephoneDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Telephone> GetByIdAsync(string composedId)
        {
            var telephone = await FindByIdAsync(composedId);

            if (telephone == null)
            {
                _logger.LogError($"No telephone found with composed id {composedId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Telephone>(telephone);
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

        public async Task<Paged<Telephone>> GetAllByBuildingIdAsync(int buildingId, QuerySet query)
        {
            return await this.GetAllByCustomerDataIdAsync(buildingId, query);
        }

        public async Task<Telephone> CreateAsync(Telephone telephone)
        {
            var telephoneRecord = _mapper.Map<DataProvider.Models.Telephone>(telephone);

            _context.Telephones.Add(telephoneRecord);

            try
            {
                await _context.SaveChangesAsync();

                return _mapper.Map<Telephone>(telephoneRecord);

            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                if (e.InnerException.Message.Contains("insert duplicate key in object"))
                    throw new EntityAlreadyExistsException();
                else
                    throw e;
            }
        }

        public async Task DeleteByIdAsync(string composedId)
        {
            var telephone = await FindByIdAsync(composedId);

            if (telephone != null)
            {
                _context.Telephones.Remove(telephone);
                await _context.SaveChangesAsync();
           }
        }

        private async Task<DataProvider.Models.Telephone> FindByIdAsync(string composedId)
        {
            var customerId = DataProvider.Models.Telephone.DecomposeCustomerRecordId(composedId);
            var telephoneTypeId = DataProvider.Models.Telephone.DecomposeTelephoneTypeId(composedId);
            var countryId = DataProvider.Models.Telephone.DecomposeCountryId(composedId);
            var area = DataProvider.Models.Telephone.DecomposeArea(composedId);
            var number = DataProvider.Models.Telephone.DecomposeNumber(composedId);

            return await _context.Telephones.Where(x => x.CustomerRecordId == customerId
                            && x.TelephoneTypeId == telephoneTypeId && x.CountryId == countryId
                            && x.Area == area && x.Number == number)
                            .FirstOrDefaultAsync();
        }

        private async Task<Paged<Telephone>> GetAllByCustomerDataIdAsync(int dataId, QuerySet query)
        {
            var source = _context.Telephones
                            .Where(t => t.CustomerRecordId == dataId)
                            .Include(query)
                            .Sort(query);

            var telephones = await source.ForPage(query).ToListAsync();

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