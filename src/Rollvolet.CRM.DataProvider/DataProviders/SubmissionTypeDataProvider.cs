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
    }
}