using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.DataProviders
{   
    public class TelephoneTypeDataProvider : ITelephoneTypeDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;

        public TelephoneTypeDataProvider(CrmContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TelephoneType>> GetAll()
        {
            var telephoneTypes = _context.TelephoneTypes.OrderBy(c => c.Name).AsEnumerable();

            return _mapper.Map<IEnumerable<TelephoneType>>(telephoneTypes);
        }

        public async Task<TelephoneType> GetByIdAsync(int id)
        {
            var telephoneType = _context.Countries.Single(x => x.Id == id);

            return _mapper.Map<TelephoneType>(telephoneType);
        }
    }
}