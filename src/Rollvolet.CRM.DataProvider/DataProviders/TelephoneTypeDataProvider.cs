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
    public class TelephoneTypeDataProvider : ITelephoneTypeDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public TelephoneTypeDataProvider(CrmContext context, IMapper mapper, ILogger<TelephoneDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TelephoneType>> GetAll()
        {
            var telephoneTypes = _context.TelephoneTypes.OrderBy(c => c.Name).AsEnumerable();

            return _mapper.Map<IEnumerable<TelephoneType>>(telephoneTypes);
        }

        public async Task<TelephoneType> GetByIdAsync(int id)
        {

            var telephoneType = await _context.TelephoneTypes.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (telephoneType == null)
            {
                _logger.LogError($"No telephone-type found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<TelephoneType>(telephoneType);
        }

        public async Task<TelephoneType> GetByTelephoneIdAsync(string composedId)
        {
            var telephoneTypeId = DataProvider.Models.Telephone.DecomposeTelephoneTypeId(composedId);
            var telephoneType = await _context.TelephoneTypes.Where(c => c.Id == telephoneTypeId).FirstOrDefaultAsync();

            if (telephoneType == null)
            {
                _logger.LogError($"No telephone-type found for telephone with id {composedId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<TelephoneType>(telephoneType);
        }
    }
}