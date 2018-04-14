using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.DataProviders
{   
    public class HonorificPrefixDataProvider : IHonorificPrefixDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;

        public HonorificPrefixDataProvider(CrmContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<HonorificPrefix>> GetAll()
        {
            var honorificPrefixes = _context.HonorificPrefixes.OrderBy(c => c.Id).AsEnumerable();

            return _mapper.Map<IEnumerable<HonorificPrefix>>(honorificPrefixes);
        }

        public async Task<HonorificPrefix> GetByIdAsync(int id)
        {
            var prefix = _context.HonorificPrefixes.Single(x => x.Id == id);

            return _mapper.Map<HonorificPrefix>(prefix);
        }
    }
}