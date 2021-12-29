using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.DataProvider.Extensions;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using System.Collections.Generic;
using Rollvolet.CRM.Domain.Contracts.MsGraph;

namespace Rollvolet.CRM.DataProviders
{
    public class SystemTaskDataProvider : ISystemTaskDataProvider
    {
        private readonly CrmContext _context;
        private readonly IGraphApiSystemTaskService _graphApiSystemTask;
        private readonly ILogger _logger;

        public SystemTaskDataProvider(CrmContext context, IGraphApiSystemTaskService graphApiSystemTask,
                                        ILogger<SystemTaskDataProvider> logger)
        {
            _context = context;
            _graphApiSystemTask = graphApiSystemTask;
            _logger = logger;
        }

        public async Task RenameOfferDocuments(string[] locations)
        {
            foreach (var location in locations)
            {
                for (var year = 1995; year < 2022; year++)
                {
                    var directory = $"{location}/{year}";
                    _logger.LogInformation($"Start renaming documents in folder {directory}");
                    await _graphApiSystemTask.RenameOfferDocumentsAsync(directory);
                }
            }
        }

        public async Task RecalcalulateSearchNames()
        {
            await RecalcalulateCustomerSearchNames();
            await RecalcalulateContactSearchNames();
            await RecalcalulateBuildingSearchNames();
        }

        private async Task RecalcalulateCustomerSearchNames()
        {
            var total = await _context.Customers.CountAsync();

            var query = new QuerySet();
            query.Page.Number = 0;
            query.Page.Size = 1000;

            _logger.LogInformation($"Updating search names of {total} customers");
            while (query.Page.Number * query.Page.Size < total)
            {
                var customers = _context.Customers.ForPage(query).AsNoTracking().Select(c => new { Id = c.DataId, Name = c.Name }).ToList();
                await UpdateBatch(customers);
                query.Page.Number += 1;
                var handledCount = query.Page.Number * query.Page.Size > total ? total : query.Page.Number * query.Page.Size;
                _logger.LogInformation($"Updated {handledCount}/{total} customers");
            }
        }

        private async Task RecalcalulateContactSearchNames()
        {
            var total = await _context.Contacts.CountAsync();

            var query = new QuerySet();
            query.Page.Number = 0;
            query.Page.Size = 1000;

            _logger.LogInformation($"Updating search names of {total} contacts");
            while (query.Page.Number * query.Page.Size < total)
            {
                var contacts = _context.Contacts.ForPage(query).AsNoTracking().Select(c => new { Id = c.DataId, Name = c.Name }).ToList();
                await UpdateBatch(contacts);
                query.Page.Number += 1;
                var handledCount = query.Page.Number * query.Page.Size > total ? total : query.Page.Number * query.Page.Size;
                _logger.LogInformation($"Updated {handledCount}/{total} contacts");
            }
        }

        private async Task RecalcalulateBuildingSearchNames()
        {
            var total = await _context.Buildings.CountAsync();

            var query = new QuerySet();
            query.Page.Number = 0;
            query.Page.Size = 1000;

            _logger.LogInformation($"Updating search names of {total} buildings");
            while (query.Page.Number * query.Page.Size < total)
            {
                var buildings = _context.Buildings.ForPage(query).AsNoTracking().Select(c => new { Id = c.DataId, Name = c.Name }).ToList();
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
                await _context.Database.ExecuteSqlRawAsync($"UPDATE dbo.tblData SET ZoekNaam = {searchName} FROM dbo.tblData AS [c] WHERE c.DataID = {record.Id}");
            }
        }

    }
}