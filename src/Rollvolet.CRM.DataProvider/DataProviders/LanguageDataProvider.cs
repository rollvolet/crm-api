using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.DataProviders
{   
    public class LanguageDataProvider : ILanguageDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;

        public LanguageDataProvider(CrmContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Language> GetByIdAsync(int id)
        {
            var language = _context.Languages.Single(x => x.Id == id);

            return _mapper.Map<Language>(language);
        }
    }
}