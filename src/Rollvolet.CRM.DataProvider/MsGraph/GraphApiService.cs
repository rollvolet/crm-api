
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Rollvolet.CRM.Domain.Configuration;
using Rollvolet.CRM.Domain.Contracts.MsGraph;
using Rollvolet.CRM.Domain.Models;
using System;
using System.Threading.Tasks;

namespace Rollvolet.CRM.DataProvider.MsGraph
{
    public class GraphApiService : IGraphApiService
    {
        private readonly int visitStartTime = 17;

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

                // TODO create new field to save office365 Id
                visit.CalendarId = calendarEvent.Id;
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
            if (visit.CalendarId != null)
            {
                if (visit.VisitDate != null)
                {
                    var updatedEvent = CreateEvent(visit, customerEntity);
                    updatedEvent = await _client.Users[_calendarConfig.KlantenbezoekCalendarId].Calendar.Events[visit.CalendarId]
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
                _logger.LogWarning("Cannot update calendar event for visit {0} since calendarId is not set on the visit.", visit.Id);
            }

            return visit;
        }

        public async Task<Visit> DeleteCalendarEventForVisit(Visit visit)
        {
            if (visit.CalendarId != null)
            {
                await _client.Users[_calendarConfig.KlantenbezoekCalendarId].Calendar.Events[visit.CalendarId].Request().DeleteAsync();
                visit.CalendarId = null;
                visit.CalendarSubject = null;
            }
            else
            {
                _logger.LogWarning("Cannot delete calendar event for visit {0} since calendarId is not set on the visit.", visit.Id);
            }

            return visit;
        }

        private Event CreateEvent(Visit visit, CustomerEntity customerEntity)
        {
            var visitDate = (DateTime) visit.VisitDate;
            var year = visitDate.Year + 1; // TODO remove +1
            var month = visitDate.Month;
            var day = visitDate.Day;

            // TODO add period to subject
            var subject = $"{customerEntity.Name} {customerEntity.PostalCode} {customerEntity.City} ({visit.Request.Comment})";

            var start = new DateTimeTimeZone() {
                DateTime = new DateTime(year, month, day, visitStartTime, 0, 0).ToString("o"),
                TimeZone = "Romance Standard Time"
            };
            var end = new DateTimeTimeZone() {
                DateTime = new DateTime(year, month, day, visitStartTime + 1, 0, 0).ToString("o"),
                TimeZone = "Romance Standard Time"
            };

            return new Event
            {
                Subject = subject,
                Start = start,
                End = end
            };
        }
    }
}