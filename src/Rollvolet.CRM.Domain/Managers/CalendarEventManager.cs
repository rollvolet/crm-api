using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Contracts.MsGraph;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class CalendarEventManager : ICalendarEventManager
    {
        private readonly Regex periodStiptUur = new Regex("^([\\d\\.:,])*(\\s)*uur\\s(stipt)");
        private readonly Regex periodBepaaldUur = new Regex("^([\\d\\.:,])*\\suur");
        private readonly Regex periodVanTot = new Regex("^[\\d\\.:,]*-[\\d\\.:,]*\\s");

        private readonly IVisitDataProvider _visitDataProvider;
        private readonly IRequestDataProvider _requestDataProvider;
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly IGraphApiService _graphApiService;
        private readonly ILogger _logger;

        public CalendarEventManager(IVisitDataProvider visitDataProvider, IRequestDataProvider requestDataProvider, ICustomerDataProvider customerDataProvider,
                            IGraphApiService graphApiService, ILogger<CalendarEventManager> logger)
        {
            _visitDataProvider = visitDataProvider;
            _requestDataProvider = requestDataProvider;
            _customerDataProvider = customerDataProvider;
            _graphApiService = graphApiService;
            _logger = logger;
        }

        public async Task<CalendarEvent> GetByIdAsync(int id, QuerySet query = null)
        {
            var calendarEvent = await _visitDataProvider.GetByIdAsync(id, query);

            if (calendarEvent == null)
            {
                _logger.LogError($"No calendar event found with id {id}");
                throw new EntityNotFoundException();
            }

            if (calendarEvent.MsObjectId != null)
                calendarEvent = await SyncSubjectAndPeriod(calendarEvent);

            return calendarEvent;
        }

        public async Task<CalendarEvent> GetByRequestIdAsync(int id)
        {
            var calendarEvent = await _visitDataProvider.GetByRequestIdAsync(id);

            if (calendarEvent == null)
            {
                _logger.LogError($"No calendar event found with id {id}");
                throw new EntityNotFoundException();
            }

            if (calendarEvent.MsObjectId != null)
                calendarEvent = await SyncSubjectAndPeriod(calendarEvent);

            return calendarEvent;
        }

        public async Task<CalendarEvent> CreateAsync(CalendarEvent calendarEvent)
        {
            // Validations
            if (calendarEvent.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Calender event cannot have an id on create.");
            if (calendarEvent.VisitDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Visit date is required on calendar event creation.");
            if (calendarEvent.Period == null)
                throw new IllegalArgumentException("IllegalAttribute", "Period is required on calendar event creation.");
            if (calendarEvent.RequiresFromHour && calendarEvent.FromHour == null)
                throw new IllegalArgumentException("IllegalAttribute", $"FromHour is required on calendar event for period {calendarEvent.Period}.");
            if (calendarEvent.RequiresUntilHour && calendarEvent.UntilHour == null)
                throw new IllegalArgumentException("IllegalAttribute", $"UntilHour is required on calendar event for period {calendarEvent.Period}.");
            if (calendarEvent.Request == null)
                throw new IllegalArgumentException("IllegalAttribute", "Request is required on calendar event creation.");
            if (calendarEvent.CalendarId != null || calendarEvent.MsObjectId != null || calendarEvent.CalendarId != null)
                throw new IllegalArgumentException("IllegalAttribute", "Calendar properties cannot be set.");
            if (calendarEvent.Customer != null)
            {
                var message = "Customer cannot be added to a calendar event on creation.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }

            await EmbedRelations(calendarEvent);

            var customerEntity = await GetRelatedCustomerEntity(calendarEvent);
            calendarEvent = await _graphApiService.CreateEventForRequestAsync(calendarEvent, customerEntity);

            return await _visitDataProvider.UpdateAsync(calendarEvent);
        }

        public async Task<CalendarEvent> UpdateAsync(CalendarEvent calendarEvent)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer", "request" };
            var existingCalendarEvent = await _visitDataProvider.GetByIdAsync(calendarEvent.Id, query);

            if (calendarEvent.IsMasteredByAccess)
                throw new IllegalArgumentException("IllegalAttribute", $"Calendar event {calendarEvent.Id} cannot be updated since it's mastered by Access.");
            if (calendarEvent.Id != existingCalendarEvent.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Calendar event id cannot be updated.");
            if (calendarEvent.VisitDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Visit date is required on calendar event.");
            if (calendarEvent.Period == null)
                throw new IllegalArgumentException("IllegalAttribute", "Period is required on calendar event creation.");
            if (calendarEvent.RequiresFromHour && calendarEvent.FromHour == null)
                throw new IllegalArgumentException("IllegalAttribute", $"FromHour is required on calendar event for period {calendarEvent.Period}.");
            if (calendarEvent.RequiresUntilHour && calendarEvent.UntilHour == null)
                throw new IllegalArgumentException("IllegalAttribute", $"UntilHour is required on calendar event for period {calendarEvent.Period}.");
            if (calendarEvent.Customer != null)
            {
                var message = "Customer cannot be set on a calendar event.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }
            if (calendarEvent.CalendarId != existingCalendarEvent.CalendarId || calendarEvent.MsObjectId != existingCalendarEvent.MsObjectId || calendarEvent.CalendarSubject != existingCalendarEvent.CalendarSubject)
                throw new IllegalArgumentException("IllegalAttribute", "Calendar properties cannot be updated.");

            await EmbedRelations(calendarEvent, existingCalendarEvent);

            if (RequiresCalendarEventUpdate(existingCalendarEvent, calendarEvent))
            {
                var customerEntity = await GetRelatedCustomerEntity(calendarEvent);
                calendarEvent = await _graphApiService.UpdateEventForRequestAsync(calendarEvent, customerEntity);
            }

            return await _visitDataProvider.UpdateAsync(calendarEvent);
        }

        public async Task DeleteAsync(int id)
        {
            var calendarEvent = await _visitDataProvider.GetByIdAsync(id);

            calendarEvent = await _graphApiService.DeleteEventForRequestAsync(calendarEvent);

            calendarEvent.VisitDate = null;
            calendarEvent.Comment = null;
            await _visitDataProvider.UpdateAsync(calendarEvent);
        }

        // Embed relations in calendar event: reuse old relation if there is one and it hasn't changed
        private async Task EmbedRelations(CalendarEvent calendarEvent, CalendarEvent oldCalendarEvent = null)
        {
            try {
                // Request cannot be updated. Take request of oldCalendarEvent on update.
                if (oldCalendarEvent != null)
                    calendarEvent.Request = oldCalendarEvent.Request;
                else
                    calendarEvent.Request = await _requestDataProvider.GetByIdAsync(calendarEvent.Request.Id);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }

        private async Task<CustomerEntity> GetRelatedCustomerEntity(CalendarEvent calendarEvent)
        {
            // TODO must this be the building if there is one?
            return await _customerDataProvider.GetByRequestIdAsync(calendarEvent.Request.Id);
        }

        private async Task<CalendarEvent> SyncSubjectAndPeriod(CalendarEvent calendarEvent)
        {
            var subject = await _graphApiService.GetVisitSubjectAsync(calendarEvent.MsObjectId);

            if (calendarEvent.CalendarSubject != subject)
            {
                calendarEvent.CalendarSubject = subject;
                calendarEvent = await _visitDataProvider.UpdateAsync(calendarEvent);
            }

            ParsePeriodFromSubject(calendarEvent);

            return calendarEvent;
        }

        private void ParsePeriodFromSubject(CalendarEvent calendarEvent)
        {
            var subject = calendarEvent.CalendarSubject;

            if (subject != null)
            {
                if (subject.StartsWith("GD") || subject.StartsWith("VM") || subject.StartsWith("NM"))
                {
                    calendarEvent.Period = subject.Substring(0, 2);
                    calendarEvent.FromHour = null;
                    calendarEvent.UntilHour = null;
                }
                else if (subject.StartsWith("vanaf"))
                {
                    calendarEvent.Period = "vanaf";
                    calendarEvent.FromHour = subject.Substring("vanaf".Length).Split("uur").FirstOrDefault().Trim();
                    calendarEvent.UntilHour = null;
                }
                else if (subject.StartsWith("rond"))
                {
                    calendarEvent.Period = "benaderend uur";
                    calendarEvent.FromHour = subject.Substring("rond".Length).Split("uur").FirstOrDefault().Trim();
                    calendarEvent.UntilHour = null;
                }
                else if (periodStiptUur.IsMatch(subject))
                {
                    calendarEvent.Period = "stipt uur";
                    calendarEvent.FromHour = subject.Split("uur").FirstOrDefault().Trim();
                    calendarEvent.UntilHour = null;
                }
                else if (periodBepaaldUur.IsMatch(subject))
                {
                    calendarEvent.Period = "bepaald uur";
                    calendarEvent.FromHour = subject.Split("uur").FirstOrDefault().Trim();
                    calendarEvent.UntilHour = null;
                }
                else if (periodVanTot.IsMatch(subject))
                {
                    calendarEvent.Period = "van-tot";
                    var timeRangeSeparatorIndex = subject.IndexOf('-');
                    calendarEvent.FromHour = subject.Substring(0, timeRangeSeparatorIndex).Trim();
                    var timeRangeEndIndex = subject.IndexOf(" ");
                    calendarEvent.UntilHour = subject.Substring(timeRangeSeparatorIndex + 1, timeRangeEndIndex - timeRangeSeparatorIndex);
                }
                else
                {
                    _logger.LogWarning("Unable to parse period from subject {0} for visit {1} and event {2}", subject, calendarEvent.Id, calendarEvent.MsObjectId);
                    calendarEvent.Period = null;
                    calendarEvent.FromHour = null;
                    calendarEvent.UntilHour = null;
                }
            }
            else
            {
                _logger.LogWarning("Visit doesn't have a subject. Unable to parse period for visit {0} and event {1}", calendarEvent.Id, calendarEvent.MsObjectId);
                calendarEvent.Period = null;
                calendarEvent.FromHour = null;
                calendarEvent.UntilHour = null;
            }

        }

        private bool RequiresCalendarEventUpdate(CalendarEvent existingCalendarEvent, CalendarEvent calendarEvent)
        {
            ParsePeriodFromSubject(existingCalendarEvent);

            return calendarEvent.VisitDate != existingCalendarEvent.VisitDate
                || calendarEvent.Period != existingCalendarEvent.Period
                || calendarEvent.FromHour != existingCalendarEvent.FromHour
                || calendarEvent.UntilHour != existingCalendarEvent.UntilHour
                || calendarEvent.Request.Comment != existingCalendarEvent.Comment;
        }
    }
}