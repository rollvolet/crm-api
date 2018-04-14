using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.DataProviders
{   
    public class CountryDataProvider : ICountryDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;

        public CountryDataProvider(CrmContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Country>> GetAll()
        {
            var countries = _context.Countries.OrderBy(c => c.Name).AsEnumerable();

            return _mapper.Map<IEnumerable<Country>>(countries);
        }

        public async Task<Country> GetByIdAsync(int id)
        {
            var country = _context.Countries.Single(x => x.Id == id);

            return _mapper.Map<Country>(country);
        }
    }
}