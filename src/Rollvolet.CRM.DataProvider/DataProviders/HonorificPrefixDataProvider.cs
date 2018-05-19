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

        public async Task<HonorificPrefix> GetByCustomerNumberAsync(int number)
        {
            var honorificPrefix = await _context.Customers.Where(c => c.Number == number).Select(c => c.HonorificPrefix).FirstOrDefaultAsync();

            if (honorificPrefix == null)
            {
                _logger.LogError($"No honorific-prefix found for customer with number {number}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<HonorificPrefix>(honorificPrefix);
        }

        public async Task<HonorificPrefix> GetByContactIdAsync(int id)
        {
            var honorificPrefix = await _context.Contacts.Where(c => c.DataId == id).Select(c => c.HonorificPrefix).FirstOrDefaultAsync();

            if (honorificPrefix == null)
            {
                _logger.LogError($"No honorific-prefix found for contact with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<HonorificPrefix>(honorificPrefix);
        }

        public async Task<HonorificPrefix> GetByBuildingIdAsync(int id)
        {
            var honorificPrefix = await _context.Buildings.Where(c => c.DataId == id).Select(c => c.HonorificPrefix).FirstOrDefaultAsync();

            if (honorificPrefix == null)
            {
                _logger.LogError($"No honorific-prefix found for building with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<HonorificPrefix>(honorificPrefix);
        }
    }
}