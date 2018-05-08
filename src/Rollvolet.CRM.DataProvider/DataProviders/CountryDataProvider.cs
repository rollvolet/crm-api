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

        public async Task<IEnumerable<Country>> GetAll()
        {
            var countries = _context.Countries.OrderBy(c => c.Name).AsEnumerable();

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
    }
}