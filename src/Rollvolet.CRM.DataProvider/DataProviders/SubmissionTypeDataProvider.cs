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
    public class SubmissionTypeDataProvider : ISubmissionTypeDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public SubmissionTypeDataProvider(CrmContext context, IMapper mapper, ILogger<SubmissionTypeDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SubmissionType>> GetAll()
        {
            var submissionTypes = _context.SubmissionTypes.OrderBy(c => c.Order).AsEnumerable();

            return _mapper.Map<IEnumerable<SubmissionType>>(submissionTypes);
        }

        public async Task<SubmissionType> GetByIdAsync(string id)
        {
            var submissionType = await _context.SubmissionTypes.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (submissionType == null)
            {
                _logger.LogError($"No submission-type found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<SubmissionType>(submissionType);
        }

        public async Task<SubmissionType> GetByOfferIdAsync(int id)
        {
            var submissionType = await _context.Offers.Where(c => c.Id == id).Select(c => c.SubmissionType).FirstOrDefaultAsync();

            if (submissionType == null)
            {
                _logger.LogError($"No submission-type found for offer with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<SubmissionType>(submissionType);
        }
    }
}