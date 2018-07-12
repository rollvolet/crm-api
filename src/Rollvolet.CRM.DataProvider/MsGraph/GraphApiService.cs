
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Rollvolet.CRM.Domain.Configuration;
using Rollvolet.CRM.Domain.Contracts.MsGraph;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rollvolet.CRM.DataProvider.MsGraph
{
    public class GraphApiService : IGraphApiService
    {
        private readonly int VISIT_START_TIME = 17;

        private readonly IGraphServiceClient _client;
        private readonly CalendarConfiguration _calendarConfig;
        private readonly ILogger _logger;

        public GraphApiService(IAuthenticationProvider authenticationProvider, IOptions<CalendarConfiguration> calendarConfiguration,
                                 ILogger<GraphApiService> logger)
        {
            _client = new GraphServiceClient(authenticationProvider);
            _calendarConfig = calendarConfiguration.Value;
            _logger = logger;
        }

        public async Task<Visit> CreateCalendarEventForVisit(Visit visit, CustomerEntity customerEntity)
        {
            if (visit.VisitDate != null)
            {
                var calendarEvent = CreateEvent(visit, customerEntity);
                calendarEvent = await _client.Users[_calendarConfig.KlantenbezoekCalendarId].Calendar.Events.Request().AddAsync(calendarEvent);

                visit.MsObjectId = calendarEvent.Id;
                visit.CalendarSubject = calendarEvent.Subject;
                _logger.LogDebug("Created calendar event in calendar {0} with id {1}", _calendarConfig.KlantenbezoekCalendarId, calendarEvent.Id);
            }
            else
            {
                _logger.LogWarning("Unable to create visit {0} in calendar. Visit date is missing.", visit.Id);
            }

            return visit;
        }

        public async Task<Visit> UpdateCalendarEventForVisit(Visit visit, CustomerEntity customerEntity)
        {
            if (visit.MsObjectId != null)
            {
                if (visit.VisitDate != null)
                {
                    var updatedEvent = CreateEvent(visit, customerEntity);
                    updatedEvent = await _client.Users[_calendarConfig.KlantenbezoekCalendarId].Calendar.Events[visit.MsObjectId]
                                    .Request().UpdateAsync(updatedEvent);

                    visit.CalendarSubject = updatedEvent.Subject;

                    _logger.LogDebug("Updated calendar event in calendar {0} with id {1}", _calendarConfig.KlantenbezoekCalendarId, updatedEvent.Id);
                }
                else
                {
                    _logger.LogWarning("Unable to update visit {0} in calendar. Visit date is missing.", visit.Id);
                }
            }
            else
            {
                _logger.LogWarning("Cannot update calendar event for visit {0} since ms-object-id is not set on the visit.", visit.Id);
            }

            return visit;
        }

        public async Task<Visit> DeleteCalendarEventForVisit(Visit visit)
        {
            if (visit.MsObjectId != null)
            {
                await _client.Users[_calendarConfig.KlantenbezoekCalendarId].Calendar.Events[visit.MsObjectId].Request().DeleteAsync();
                visit.CalendarId = null;
                visit.MsObjectId = null;
                visit.CalendarSubject = null;
            }
            else
            {
                _logger.LogWarning("Cannot delete calendar event for visit {0} since ms-object-id is not set on the visit.", visit.Id);
            }

            return visit;
        }

        public async Task<string> GetSubject(string msObjectId)
        {
            var calendarEvent = await _client.Users[_calendarConfig.KlantenbezoekCalendarId].Calendar.Events[msObjectId].Request().GetAsync();

            if (calendarEvent != null)
                return calendarEvent.Subject;
            else
                throw new EntityNotFoundException();
        }

        private Event CreateEvent(Visit visit, CustomerEntity customerEntity)
        {
            var visitDate = (DateTime) visit.VisitDate;
            var year = visitDate.Year + 1; // TODO remove +1
            var month = visitDate.Month;
            var day = visitDate.Day;
            var period = GetPeriodMessage(visit.Period, visit.FromHour, visit.UntilHour);

            var subject = $"{period} {customerEntity.Name} {customerEntity.PostalCode} {customerEntity.City} ({visit.Request.Comment})";

            var start = new DateTimeTimeZone() {
                DateTime = new DateTime(year, month, day, VISIT_START_TIME, 0, 0).ToString("o"),
                TimeZone = "Romance Standard Time"
            };
            var end = new DateTimeTimeZone() {
                DateTime = new DateTime(year, month, day, VISIT_START_TIME + 1, 0, 0).ToString("o"),
                TimeZone = "Romance Standard Time"
            };

            return new Event
            {
                Subject = subject,
                Start = start,
                End = end
            };
        }

        private string GetPeriodMessage(string period, string from, string until)
        {
            if (period == "GD" || period == "VM" || period == "NM")
                return period;
            else if (period == "vanaf")
                return $"vanaf {from} uur";
            else if (period == "benaderend uur")
                return $"rond {from} uur";
            else if (period == "stipt uur")
                return $"{from} uur (stipt)";
            else if (period == "bepaald uur")
                return $"{from} uur";
            else if (period == "van-tot")
                return $"{from}-{until}";

            _logger.LogWarning("Cannot format period message for given period '{0}'", period);
            return "";
        }
    }
}
