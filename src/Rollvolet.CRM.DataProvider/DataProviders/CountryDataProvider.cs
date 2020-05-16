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
    public class CountryDataProvider : ICountryDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CountryDataProvider(CrmContext context, IMapper mapper, ILogger<CountryDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            var countries = await _context.Countries.OrderBy(c => c.Name).ToListAsync();

            return _mapper.Map<IEnumerable<Country>>(countries);
        }

        public async Task<Country> GetByIdAsync(int id)
        {
            var country = await _context.Countries.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (country == null)
            {
                _logger.LogError($"No country found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Country>(country);
        }

        public async Task<Country> GetByCustomerNumberAsync(int number)
        {
            var country = await _context.Customers.Where(c => c.Number == number).Select(c => c.Country).FirstOrDefaultAsync();

            if (country == null)
            {
                _logger.LogError($"No country found for customer with number {number}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Country>(country);
        }

        public async Task<Country> GetByContactIdAsync(int id)
        {
            var country = await _context.Contacts.Where(c => c.DataId == id).Select(c => c.Country).FirstOrDefaultAsync();

            if (country == null)
            {
                _logger.LogError($"No country found for contact with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Country>(country);
        }

        public async Task<Country> GetByBuildingIdAsync(int id)
        {
            var country = await _context.Buildings.Where(c => c.DataId == id).Select(c => c.Country).FirstOrDefaultAsync();

            if (country == null)
            {
                _logger.LogError($"No country found for building with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Country>(country);
        }

        public async Task<Country> GetByTelephoneIdAsync(string composedId)
        {
            var countryId = DataProvider.Models.Telephone.DecomposeCountryId(composedId);
            var country = await _context.Countries.Where(c => c.Id == countryId).FirstOrDefaultAsync();

            if (country == null)
            {
                _logger.LogError($"No country found for telephone with id {composedId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Country>(country);
        }
    }
}