using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Extensions;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProviders
{
    public class AccountancyExportDataProvider : IAccountancyExportDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AccountancyExportDataProvider(CrmContext context, IMapper mapper, ILogger<AccountancyExportDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Paged<AccountancyExport>> GetAllAsync(QuerySet query)
        {
            var source = _context.AccountancyExports
                            .Sort(query)
                            .Filter(query);

            var accountancyExports = source.ForPage(query).AsEnumerable();

            var mappedAccountancyExports = _mapper.Map<IEnumerable<AccountancyExport>>(accountancyExports);

            var count = await source.CountAsync();

            return new Paged<AccountancyExport>() {
                Items = mappedAccountancyExports,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<AccountancyExport> GetByIdAsync(int id, QuerySet query = null)
        {
            var accountancyExport = await _context.AccountancyExports.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (accountancyExport == null)
            {
                _logger.LogError($"No accountancy-export found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<AccountancyExport>(accountancyExport);
        }

        public async Task<AccountancyExport> CreateAsync(AccountancyExport accountancyExport)
        {
            var accountancyExportRecord = _mapper.Map<DataProvider.Models.AccountancyExport>(accountancyExport);

            _context.AccountancyExports.Add(accountancyExportRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<AccountancyExport>(accountancyExportRecord);
        }
    }
}