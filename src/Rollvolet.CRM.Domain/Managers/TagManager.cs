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
    public class TagManager : ITagManager
    {
        private readonly ITagDataProvider _tagDataProvider;
        private readonly ILogger _logger;

        public TagManager(ITagDataProvider tagDataProvider, ILogger<TagManager> logger)
        {
            _tagDataProvider = tagDataProvider;
            _logger = logger;
        }

        public async Task<Paged<Tag>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            return await _tagDataProvider.GetAllByCustomerNumberAsync(customerId, query);
        }
    }
}