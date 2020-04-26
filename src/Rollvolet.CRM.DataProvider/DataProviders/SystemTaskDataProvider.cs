using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.DataProvider.Extensions;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using System.Collections.Generic;

namespace Rollvolet.CRM.DataProviders
{
    public class SystemTaskDataProvider : ISystemTaskDataProvider
    {
        private readonly CrmContext _context;
        private readonly ILogger _logger;

        public SystemTaskDataProvider(CrmContext context, ILogger<SystemTaskDataProvider> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RecalcalulateSearchNames()
        {
            await RecalcalulateCustomerSearchNames();
            await RecalcalulateContactSearchNames();
            await RecalcalulateBuildingSearchNames();
        }

        public async Task RecalcalulateCustomerSearchNames()
        {
            var total = await _context.Customers.CountAsync();

            var query = new QuerySet();
            query.Page.Number = 0;
            query.Page.Size = 1000;

            _logger.LogInformation($"Updating search names of {total} customers");
            while (query.Page.Number * query.Page.Size < total)
            {
                var customers = _context.Customers.ForPage(query).AsNoTracking().Select(c => new { Id = c.DataId, Name = c.Name }).AsEnumerable();
                await UpdateBatch(customers);
                query.Page.Number += 1;
                var handledCount = query.Page.Number * query.Page.Size > total ? total : query.Page.Number * query.Page.Size;
                _logger.LogInformation($"Updated {handledCount}/{total} customers");
            }
        }

        public async Task RecalcalulateContactSearchNames()
        {
            var total = await _context.Contacts.CountAsync();

            var query = new QuerySet();
            query.Page.Number = 0;
            query.Page.Size = 1000;

            _logger.LogInformation($"Updating search names of {total} contacts");
            while (query.Page.Number * query.Page.Size < total)
            {
                var contacts = _context.Contacts.ForPage(query).AsNoTracking().Select(c => new { Id = c.DataId, Name = c.Name }).AsEnumerable();
                await UpdateBatch(contacts);
                query.Page.Number += 1;
                var handledCount = query.Page.Number * query.Page.Size > total ? total : query.Page.Number * query.Page.Size;
                _logger.LogInformation($"Updated {handledCount}/{total} contacts");
            }
        }

        public async Task RecalcalulateBuildingSearchNames()
        {
            var total = await _context.Buildings.CountAsync();

            var query = new QuerySet();
            query.Page.Number = 0;
            query.Page.Size = 1000;

            _logger.LogInformation($"Updating search names of {total} buildings");
            while (query.Page.Number * query.Page.Size < total)
            {
                var buildings = _context.Buildings.ForPage(query).AsNoTracking().Select(c => new { Id = c.DataId, Name = c.Name }).AsEnumerable();
                await UpdateBatch(buildings);
                query.Page.Number += 1;
                var handledCount = query.Page.Number * query.Page.Size > total ? total : query.Page.Number * query.Page.Size;
                _logger.LogInformation($"Updated {handledCount}/{total} buildings");
            }
        }

        private async Task UpdateBatch(IEnumerable<dynamic> records)
        {
            foreach (var record in records)
            {
                var searchName = DataProvider.Models.CustomerRecord.CalculateSearchName(record.Name);
                await _context.Database.ExecuteSqlCommandAsync($"UPDATE dbo.tblData SET ZoekNaam = {searchName} FROM dbo.tblData AS [c] WHERE c.DataID = {record.Id}");
            }
        }

    }
}