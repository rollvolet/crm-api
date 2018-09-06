
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
        }

        public async Task<Visit> CreateCalendarEventForVisit(Visit visit, CustomerEntity customerEntity)
        {
            if (visit.VisitDate != null)
            {
                var calendarEvent = CreateVisitEvent(visit, customerEntity);
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
                    var updatedEvent = CreateVisitEvent(visit, customerEntity);
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
                _logger.LogDebug("Delete visit event in calendar {0} with id {1}", _calendarConfig.KlantenbezoekCalendarId, visit.MsObjectId);
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

        public async Task<Order> CreateCalendarEventForPlanning(Order order)
        {
            if (order.PlanningDate != null)
            {
                var calendarEvent = await CreatePlanningEvent(order);
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

        public async Task<Order> UpdateCalendarEventForPlanning(Order order)
        {
            if (order.PlanningMsObjectId != null)
            {
                if (order.PlanningDate != null)
                {
                    var updatedEvent = await CreatePlanningEvent(order);
                    try
                    {
                        updatedEvent = await _client.Users[_calendarConfig.PlanningCalendarId].Calendar.Events[order.PlanningMsObjectId]
                                    .Request().UpdateAsync(updatedEvent);
                    }
                    catch (Exception)
                    {
                        _logger.LogWarning("Something went wrong while updating planning event {0} for order {1}. Event will be decoupled.", order.PlanningMsObjectId, order.Id);
                        return await DeleteCalendarEventForPlanning(order);
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

        public async Task<Order> DeleteCalendarEventForPlanning(Order order)
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

        public async Task<string> GetVisitSubject(string msObjectId)
        {
            return await GetSubject(_calendarConfig.KlantenbezoekCalendarId, msObjectId);
        }

        public async Task<string> GetPlanningSubject(string msObjectId)
        {
            return await GetSubject(_calendarConfig.PlanningCalendarId, msObjectId);
        }

        private Event CreateVisitEvent(Visit visit, CustomerEntity customerEntity)
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

        private async Task<Event> CreatePlanningEvent(Order order)
        {
            var entity = order.Building != null ? (CustomerEntity) order.Building : order.Customer;
            var addressLines = string.Join(",", new string[3] { entity.Address1, entity.Address2, entity.Address3 }.Where(a => !String.IsNullOrEmpty(a)));
            var address = $"{order.Customer.PostalCode} {order.Customer.City} ({addressLines})";

            var visitorInitials = "";
            try
            {
                var visitor = await _employeeDataProvider.GetVisitorByOrderId(order.Id);
                visitorInitials = visitor.Initials;
            }
            catch (EntityNotFoundException)
            {
                // No employee found. Nothing should happen.
            }

            var subject = $"{order.Customer.Name} - {address} ** {order.OfferNumber} - ({visitorInitials}) - {order.ScheduledHours}*{order.ScheduledNbOfPersons}";

            var planningDate = (DateTime) order.PlanningDate;
            var year = planningDate.Year + 1; // TODO remove +1
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

        private async Task<string> GetSubject(string calendarId, string msObjectId)
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
