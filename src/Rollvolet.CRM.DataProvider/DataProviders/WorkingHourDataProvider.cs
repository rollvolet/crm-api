using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.DataProvider.Extensions;
using Rollvolet.CRM.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Rollvolet.CRM.DataProviders
{
    public class WorkingHourDataProvider : IWorkingHourDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        protected readonly ILogger _logger;

        public WorkingHourDataProvider(CrmContext context, IMapper mapper, ILogger<WorkingHourDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<WorkingHour> GetByIdAsync(int id, QuerySet query = null)
        {
            var workingHour = await FindByIdAsync(id, query);

            if (workingHour == null)
            {
                _logger.LogError($"No working-hour found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<WorkingHour>(workingHour);
        }

        public async Task<Paged<WorkingHour>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query)
        {
            var source = _context.WorkingHours
                            .Where(c => c.InvoiceId == invoiceId)
                            .Include(query)
                            .Sort(query)
                            .Filter(query);

            var workingHours = await source.ForPage(query).ToListAsync();

            var count = await source.CountAsync();

            var mappedWorkingHours = _mapper.Map<IEnumerable<WorkingHour>>(workingHours);

            return new Paged<WorkingHour>() {
                Items = mappedWorkingHours,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<WorkingHour> CreateAsync(WorkingHour workingHour)
        {
            var workingHourRecord = _mapper.Map<DataProvider.Models.WorkingHour>(workingHour);

            workingHourRecord.Hours = 1;

            _context.WorkingHours.Add(workingHourRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<WorkingHour>(workingHourRecord);
        }

        public async Task<WorkingHour> UpdateAsync(WorkingHour workingHour)
        {
            var workingHourRecord = await FindByIdAsync(workingHour.Id);
            _mapper.Map(workingHour, workingHourRecord);

            _context.WorkingHours.Update(workingHourRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<WorkingHour>(workingHourRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var workingHour = await FindByIdAsync(id);

            if (workingHour != null)
            {
                _context.WorkingHours.Remove(workingHour);
                await _context.SaveChangesAsync();
           }
        }

        private async Task<DataProvider.Models.WorkingHour> FindByIdAsync(int id, QuerySet query = null)
        {
            return await FindWhereAsync(c => c.Id == id, query);
        }

        private async Task<DataProvider.Models.WorkingHour> FindWhereAsync(Expression<Func<DataProvider.Models.WorkingHour, bool>> where, QuerySet query = null)
        {
            var source = _context.WorkingHours.Where(where);

            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }
    }
}