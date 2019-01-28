using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class VisitDataProvider : IVisitDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public VisitDataProvider(CrmContext context, IMapper mapper, ILogger<VisitDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CalendarEvent> GetByIdAsync(int id, QuerySet query = null)
        {
            var visit = await FindByIdAsync(id, query);

            if (visit == null)
            {
                _logger.LogError($"No visit found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<CalendarEvent>(visit);
        }

        public async Task<CalendarEvent> GetByRequestIdAsync(int id)
        {
            var visit = await _context.Requests.Where(c => c.Id == id).Select(c => c.Visit).FirstOrDefaultAsync();

            if (visit == null)
            {
                _logger.LogError($"No visit found for request with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<CalendarEvent>(visit);
        }

        // Creation of a Visit is handled by the RequestDataProvider
        // Visit can only be created on creation of a new request

        public async Task<CalendarEvent> UpdateAsync(CalendarEvent calendarEvent)
        {
            DataProvider.Models.Visit visitRecord = null;
            // calendar event doesn't necessarily have an id, since it may not be created yet (while the underlying visit record already exists)
            if (calendarEvent.Id != 0)
                visitRecord = await FindByIdAsync(calendarEvent.Id);
            else
                visitRecord = await FindWhereAsync(v => v.RequestId == calendarEvent.Request.Id);
            _mapper.Map(calendarEvent, visitRecord);

            _context.Visits.Update(visitRecord);
            await _context.SaveChangesAsync();

            var savedCalendarEvent = _mapper.Map<CalendarEvent>(visitRecord);
            if (savedCalendarEvent != null) // maybe null if the domain resource 'CalendarEvent' has been removed
            {
                savedCalendarEvent.Period = calendarEvent.Period;
                savedCalendarEvent.FromHour = calendarEvent.FromHour;
                savedCalendarEvent.UntilHour = calendarEvent.UntilHour;
            }

            return savedCalendarEvent;
        }

        // Deletion of a Visit is handled by the RequestDataProvider
        // Visit can only be deleted on deletion of the corresponding request

        private async Task<DataProvider.Models.Visit> FindByIdAsync(int id, QuerySet query = null)
        {
            return await FindWhereAsync(c => c.Id == id, query);
        }

        private async Task<DataProvider.Models.Visit> FindWhereAsync(Expression<Func<DataProvider.Models.Visit, bool>> where, QuerySet query = null)
        {
            var source = (IQueryable<DataProvider.Models.Visit>) _context.Visits.Where(where);

            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }
    }
}