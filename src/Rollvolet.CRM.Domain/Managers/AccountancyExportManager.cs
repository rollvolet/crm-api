using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class AccountancyExportManager : IAccountancyExportManager
    {
        private readonly IAccountancyExportDataProvider _accountancyExportDataProvider;
        private readonly ILogger _logger;

        public AccountancyExportManager(IAccountancyExportDataProvider accountancyExportDataProvider,
                                        ILogger<AccountancyExportManager> logger)
        {
            _accountancyExportDataProvider = accountancyExportDataProvider;
            _logger = logger;
        }

        public async Task<Paged<AccountancyExport>> GetAllAsync(QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "date";
            }

            return await _accountancyExportDataProvider.GetAllAsync(query);
        }

        public async Task<AccountancyExport> GetByIdAsync(int id, QuerySet query)
        {
            return await _accountancyExportDataProvider.GetByIdAsync(id, query);
        }


        public async Task<AccountancyExport> CreateAsync(AccountancyExport accountancyExport)
        {
            if (accountancyExport.Id != null)
                throw new IllegalArgumentException("IllegalAttribute", "Accountancy-export cannot have an id on create.");
            if (accountancyExport.Date == null)
                throw new IllegalArgumentException("IllegalAttribute", "Date is required.");
            if (accountancyExport.FromNumber == null)
                throw new IllegalArgumentException("IllegalAttribute", "From-number is required.");

            var fromNumber = (int) accountancyExport.FromNumber;
            var untilNumber = accountancyExport.UntilNumber != null ? (int) accountancyExport.UntilNumber : fromNumber;

            return await _accountancyExportDataProvider.CreateAsync(accountancyExport);
        }
    }
}