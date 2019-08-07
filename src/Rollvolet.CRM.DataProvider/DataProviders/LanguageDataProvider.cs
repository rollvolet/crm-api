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
    public class LanguageDataProvider : ILanguageDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public LanguageDataProvider(CrmContext context, IMapper mapper, ILogger<LanguageDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Language>> GetAllAsync()
        {
            var languages = await Task.Run(() => _context.Languages.OrderBy(c => c.Name).AsEnumerable());

            return _mapper.Map<IEnumerable<Language>>(languages);
        }

        public async Task<Language> GetByIdAsync(int id)
        {
            var language = await _context.Languages.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (language == null)
            {
                _logger.LogError($"No language found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Language>(language);
        }

        public async Task<Language> GetByCustomerNumberAsync(int number)
        {
            var language = await _context.Customers.Where(c => c.Number == number).Select(c => c.Language).FirstOrDefaultAsync();

            if (language == null)
            {
                _logger.LogError($"No language found for customer with number {number}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Language>(language);
        }

        public async Task<Language> GetByContactIdAsync(int id)
        {
            var language = await _context.Contacts.Where(c => c.DataId == id).Select(c => c.Language).FirstOrDefaultAsync();

            if (language == null)
            {
                _logger.LogError($"No language found for contact with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Language>(language);
        }

        public async Task<Language> GetByBuildingIdAsync(int id)
        {
            var language = await _context.Buildings.Where(c => c.DataId == id).Select(c => c.Language).FirstOrDefaultAsync();

            if (language == null)
            {
                _logger.LogError($"No language found for building with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Language>(language);
        }
    }
}