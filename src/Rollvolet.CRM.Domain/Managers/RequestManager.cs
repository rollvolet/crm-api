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
    public class RequestManager : IRequestManager
    {
        private readonly IRequestDataProvider _requestDataProvider;
        private readonly ILogger _logger;

        public RequestManager(IRequestDataProvider requestDataProvider, ILogger<CustomerManager> logger)
        {
            _requestDataProvider = requestDataProvider;
            _logger = logger;
        }
        
        public async Task<Paged<Request>> GetAllAsync(QuerySet query)
        {                
            return await _requestDataProvider.GetAllAsync(query);
        }
    }
}