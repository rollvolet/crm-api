using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.DataProviders
{   
    public class PostalCodeDataProvider : IPostalCodeDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;

        public PostalCodeDataProvider(CrmContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PostalCode> GetByIdAsync(int id)
        {
            var postalCode = _context.PostalCodes.Single(x => x.Id == id);

            return _mapper.Map<PostalCode>(postalCode);
        }
    }
}