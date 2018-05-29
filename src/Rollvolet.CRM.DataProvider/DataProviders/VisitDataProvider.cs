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
    public class VisitDataProvider : IVisitDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public VisitDataProvider(CrmContext context, IMapper mapper, ILogger<VisitDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Visit> GetByRequestIdAsync(int id)
        {
            var visit = await _context.Requests.Where(c => c.Id == id).Select(c => c.Visit).FirstOrDefaultAsync();

            if (visit == null)
            {
                _logger.LogError($"No visit found for request with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Visit>(visit);
        }
    }
}