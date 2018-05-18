using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class SubmissionTypeManager : ISubmissionTypeManager
    {
        private readonly ISubmissionTypeDataProvider _submissionTypeDataProvider;
        private readonly ILogger _logger;

        public SubmissionTypeManager(ISubmissionTypeDataProvider submissionTypeDataProvider, ILogger<SubmissionTypeManager> logger)
        {
            _submissionTypeDataProvider = submissionTypeDataProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<SubmissionType>> GetAllAsync()
        {
            return await _submissionTypeDataProvider.GetAll();
        }
    }
}