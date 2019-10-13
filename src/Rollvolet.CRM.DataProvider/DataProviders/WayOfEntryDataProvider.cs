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
    public class WayOfEntryDataProvider : IWayOfEntryDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public WayOfEntryDataProvider(CrmContext context, IMapper mapper, ILogger<WayOfEntryDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<WayOfEntry>> GetAllAsync()
        {
            var wayOfEntries = await Task.Run(() => _context.WayOfEntries.OrderBy(c => c.Position).AsEnumerable());

            return _mapper.Map<IEnumerable<WayOfEntry>>(wayOfEntries);
        }

        public async Task<WayOfEntry> GetByIdAsync(int id)
        {
            var wayOfEntry = await _context.WayOfEntries.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (wayOfEntry == null)
            {
                _logger.LogError($"No way-of-entry found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<WayOfEntry>(wayOfEntry);
        }

        public async Task<WayOfEntry> GetByRequestIdAsync(int id)
        {
            var wayOfEntry = await _context.Requests.Where(c => c.Id == id).Select(c => c.WayOfEntry).FirstOrDefaultAsync();

            if (wayOfEntry == null)
            {
                _logger.LogError($"No way-of-entry found for request with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<WayOfEntry>(wayOfEntry);
        }
    }
}