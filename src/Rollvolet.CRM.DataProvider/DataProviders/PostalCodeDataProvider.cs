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
    public class PostalCodeDataProvider : IPostalCodeDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PostalCodeDataProvider(CrmContext context, IMapper mapper, ILogger<PostalCodeDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<PostalCode>> GetAllAsync()
        {
            var postalCodes = await _context.PostalCodes.OrderBy(c => c.Code).ToListAsync();

            return _mapper.Map<IEnumerable<PostalCode>>(postalCodes);
        }

        public async Task<PostalCode> GetByIdAsync(int id)
        {
            var postalCode = await _context.PostalCodes.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (postalCode == null)
            {
                _logger.LogError($"No postal code found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<PostalCode>(postalCode);
        }
    }
}