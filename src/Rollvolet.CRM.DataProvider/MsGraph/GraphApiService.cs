
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Rollvolet.CRM.Domain.Configuration;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Contracts.MsGraph;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rollvolet.CRM.DataProvider.MsGraph
{
    public class GraphApiService : IGraphApiService
    {
        private readonly int VISIT_START_TIME = 17;
        private readonly int PLANNING_START_TIME = 19;

        private readonly IGraphServiceClient _client;
        private readonly CalendarConfiguration _calendarConfig;
        private readonly IEmployeeDataProvider _employeeDataProvider;
        private readonly ILogger _logger;

        public GraphApiService(IAuthenticationProvider authenticationProvider, IOptions<CalendarConfiguration> calendarConfiguration,
                                IEmployeeDataProvider employeeDataProvider, ILogger<GraphApiService> logger)
        {
            _client = new GraphServiceClient(authenticationProvider);
            _calendarConfig = calendarConfiguration.Value;
            _employeeDataProvider = employeeDataProvider;
            _logger = logger;

            if (_calendarConfig.PostponeWithYears == null)
                _calendarConfig.PostponeWithYears = 0;
        }

        public async Task<CalendarEvent> CreateEventForRequestAsync(CalendarEvent calendarEvent, CustomerEntity customerEntity)
        {
            if (calendarEvent.VisitDate != null)
            {
                var msEvent = CreateRequestVisitEvent(calendarEvent, customerEntity);

                try
                {
                    msEvent = await _client.Users[_calendarConfig.KlantenbezoekCalendarId].Calendar.Events.Request().AddAsync(msEvent);
                }
                catch (Exception)
                {
                    _logger.LogWarning("Something went wrong while creating MS event for calendar event {1}.", calendarEvent.Id);
                }

                calendarEvent.MsObjectId = msEvent.Id;
                calendarEvent.CalendarSubject = msEvent.Subject;
                _logger.LogDebug("Created calendar event in calendar {0} with id {1}", _calendarConfig.KlantenbezoekCalendarId, msEvent.Id);
            }
            else
            {
                _logger.LogWarning("Unable to create visit {0} in calendar. Visit date is missing.", calendarEvent.Id);
            }

            return calendarEvent;
        }

        public async Task<CalendarEvent> UpdateEventForRequestAsync(CalendarEvent calendarEvent, CustomerEntity customerEntity)
        {
            if (calendarEvent.MsObjectId != null)
            {
                if (calendarEvent.VisitDate != null)
                {
                    var updatedMsEvent = CreateRequestVisitEvent(calendarEvent, customerEntity);
                    try
                    {
                        updatedMsEvent = await _client.Users[_calendarConfig.KlantenbezoekCalendarId].Calendar.Events[calendarEvent.MsObjectId]
                                    .Request().UpdateAsync(updatedMsEvent);
                    }
                    catch (Exception)
                    {
                        _logger.LogWarning("Something went wrong while updating MS event {0} for calendar event {1}. Event will be decoupled.", calendarEvent.MsObjectId, calendarEvent.Id);
                        return await DeleteEventForRequestAsync(calendarEvent);
                    }

                    calendarEvent.CalendarSubject = updatedMsEvent.Subject;

                    _logger.LogDebug("Updated MS event in calendar {0} with id {1}", _calendarConfig.KlantenbezoekCalendarId, updatedMsEvent.Id);
                }
                else
                {
                    _logger.LogWarning("Unable to update calendar event {0} in calendar. Visit date is missing.", calendarEvent.Id);
                }
            }
            else
            {
                _logger.LogWarning("Cannot update MS event for calendar event {0} since ms-object-id is not set on the calendar event.", calendarEvent.Id);
            }

            return calendarEvent;
        }

        public async Task<CalendarEvent> DeleteEventForRequestAsync(CalendarEvent calendarEvent)
        {
            if (calendarEvent.MsObjectId != null)
            {
                try
                {
                    await _client.Users[_calendarConfig.KlantenbezoekCalendarId].Calendar.Events[calendarEvent.MsObjectId].Request().DeleteAsync();
                    _logger.LogDebug("Delete MS event in calendar {0} with id {1}", _calendarConfig.KlantenbezoekCalendarId, calendarEvent.MsObjectId);
                }
                catch (ServiceException e)
                {
                    if (e.IsMatch(GraphErrorCode.ItemNotFound.ToString()))
                        _logger.LogDebug("MS event of calendar event {0} has already bee deleted.", calendarEvent.Id);
                    else
                        throw e;
                }

                calendarEvent.CalendarId = null;
                calendarEvent.MsObjectId = null;
                calendarEvent.CalendarSubject = null;
            }
            else
            {
                _logger.LogWarning("Cannot delete MS event for calendar event {0} since ms-object-id is not set on the visit.", calendarEvent.Id);
            }

            return calendarEvent;
        }

        public async Task<Order> CreateEventForPlanningAsync(Order order)
        {
            if (order.PlanningDate != null)
            {
                var calendarEvent = await CreatePlanningEventAsync(order);
                try
                {
                    calendarEvent = await _client.Users[_calendarConfig.PlanningCalendarId].Calendar.Events.Request().AddAsync(calendarEvent);
                }
                catch (Exception)
                {
                    _logger.LogWarning("Something went wrong while creating planning event for order {1}.", order.Id);
                }

                order.PlanningMsObjectId = calendarEvent.Id;
                _logger.LogDebug("Created planning event in calendar {0} with id {1}", _calendarConfig.PlanningCalendarId, calendarEvent.Id);
            }
            else
            {
                _logger.LogWarning("Unable to create planning event for order {0} in calendar. Planning-date is missing.", order.Id);
            }

            return order;
        }

        public async Task<Order> UpdateEventForPlanningAsync(Order order)
        {
            if (order.PlanningMsObjectId != null)
            {
                if (order.PlanningDate != null)
                {
                    var updatedEvent = await CreatePlanningEventAsync(order);
                    try
                    {
                        updatedEvent = await _client.Users[_calendarConfig.PlanningCalendarId].Calendar.Events[order.PlanningMsObjectId]
                                    .Request().UpdateAsync(updatedEvent);
                    }
                    catch (Exception)
                    {
                        _logger.LogWarning("Something went wrong while updating planning event {0} for order {1}. Event will be decoupled.", order.PlanningMsObjectId, order.Id);
                        return await DeleteEventForPlanningAsync(order);
                    }

                    _logger.LogDebug("Updated planning event in calendar {0} with id {1}", _calendarConfig.PlanningCalendarId, updatedEvent.Id);
                }
                else
                {
                    _logger.LogWarning("Unable to update planning event for order {0} in calendar. Planning date is missing.", order.Id);
                }
            }
            else
            {
                _logger.LogWarning("Cannot update planning event for order {0} since planning-ms-object-id is not set on the order.", order.Id);
            }

            return order;
        }

        public async Task<Order> DeleteEventForPlanningAsync(Order order)
        {
            if (order.PlanningMsObjectId != null)
            {
                try
                {
                    await _client.Users[_calendarConfig.PlanningCalendarId].Calendar.Events[order.PlanningMsObjectId].Request().DeleteAsync();
                    _logger.LogDebug("Delete planning event in calendar {0} with id {1}", _calendarConfig.PlanningCalendarId, order.PlanningMsObjectId);
                }
                catch(Exception e) {
                    _logger.LogWarning("Something went wrong while deleting planning event {0} for order {1}. Event will be decoupled.", order.PlanningMsObjectId, order.Id, e);
                }
                order.PlanningMsObjectId = null;
                order.PlanningDate = null;
            }
            else
            {
                _logger.LogWarning("Cannot delete planning event for order {0} since planning-ms-object-id is not set on the order.", order.Id);
            }

            return order;
        }

        public async Task<string> GetVisitSubjectAsync(string msObjectId)
        {
            return await GetSubjectAsync(_calendarConfig.KlantenbezoekCalendarId, msObjectId);
        }

        public async Task<string> GetPlanningSubjectAsync(string msObjectId)
        {
            return await GetSubjectAsync(_calendarConfig.PlanningCalendarId, msObjectId);
        }

        private Event CreateRequestVisitEvent(CalendarEvent calendarEvent, CustomerEntity customerEntity)
        {
            var visitDate = (DateTime) calendarEvent.VisitDate;
            var year = visitDate.Year + (int) _calendarConfig.PostponeWithYears;
            var month = visitDate.Month;
            var day = visitDate.Day;
            var period = GetPeriodMessage(calendarEvent.Period, calendarEvent.FromHour, calendarEvent.UntilHour);

            var subject = $"{period} {customerEntity.Name} {customerEntity.PostalCode} {customerEntity.City} ({calendarEvent.Request.Comment})";

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

        private async Task<Event> CreatePlanningEventAsync(Order order)
        {
            var entity = order.Building != null ? (CustomerEntity) order.Building : order.Customer;
            var addressLines = string.Join(",", new string[3] { entity.Address1, entity.Address2, entity.Address3 }.Where(a => !String.IsNullOrEmpty(a)));
            var address = $"{order.Customer.PostalCode} {order.Customer.City} ({addressLines})";

            var visitorInitials = "";
            try
            {
                var visitor = await _employeeDataProvider.GetVisitorByOrderIdAsync(order.Id);
                visitorInitials = visitor.Initials;
            }
            catch (EntityNotFoundException)
            {
                // No employee found. Nothing should happen.
            }

            var subject = $"{order.Customer.Name} - {address} ** AD{order.RequestNumber} - ({visitorInitials}) - {order.ScheduledHours}*{order.ScheduledNbOfPersons}";

            var planningDate = (DateTime) order.PlanningDate;
            var year = planningDate.Year + (int) _calendarConfig.PostponeWithYears;
            var month = planningDate.Month;
            var day = planningDate.Day;

            var start = new DateTimeTimeZone() {
                DateTime = new DateTime(year, month, day, PLANNING_START_TIME, 0, 0).ToString("o"),
                TimeZone = "Romance Standard Time"
            };
            var end = new DateTimeTimeZone() {
                DateTime = new DateTime(year, month, day, PLANNING_START_TIME + 1, 0, 0).ToString("o"),
                TimeZone = "Romance Standard Time"
            };

            return new Event
            {
                Subject = subject,
                Start = start,
                End = end
            };
        }

        private async Task<string> GetSubjectAsync(string calendarId, string msObjectId)
        {
            var calendarEvent = await _client.Users[calendarId].Calendar.Events[msObjectId].Request().GetAsync();

            if (calendarEvent != null)
                return calendarEvent.Subject;
            else
                throw new EntityNotFoundException();
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
