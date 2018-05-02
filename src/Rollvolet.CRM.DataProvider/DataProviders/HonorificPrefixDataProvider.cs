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
    public class HonorificPrefixDataProvider : IHonorificPrefixDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public HonorificPrefixDataProvider(CrmContext context, IMapper mapper, ILogger<HonorificPrefixDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<HonorificPrefix>> GetAll()
        {
            var honorificPrefixes = _context.HonorificPrefixes.OrderBy(c => c.Id).AsEnumerable();

            return _mapper.Map<IEnumerable<HonorificPrefix>>(honorificPrefixes);
        }

        public async Task<HonorificPrefix> GetByIdAsync(string composedId)
        {
            var entityId = DataProvider.Models.HonorificPrefix.DecomposeEntityId(composedId);
            var languageId = DataProvider.Models.HonorificPrefix.DecomposeLanguageId(composedId);
            var prefix = await _context.HonorificPrefixes.Where(x => x.Id == entityId && x.LanguageId == languageId).FirstOrDefaultAsync();
            
            if (prefix == null)
            {
                _logger.LogError($"No honorific prefix found with composed id {composedId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<HonorificPrefix>(prefix);
        }
    }
}