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

        public async Task<IEnumerable<WayOfEntry>> GetAll()
        {
            var wayOfEntries = _context.WayOfEntries.OrderBy(c => c.Name).AsEnumerable();

            return _mapper.Map<IEnumerable<WayOfEntry>>(wayOfEntries);
        }
    }
}