using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Rollvolet.CRM.Domain.Configuration;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Contracts.MsGraph;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rollvolet.CRM.DataProvider.MsGraph
{
    public class GraphApiCalendarService : IGraphApiCalendarService
    {
        private readonly int VISIT_START_TIME = 17;
        private readonly int PLANNING_START_TIME = 19;
        private readonly string INTERVENTION_SUBJECT_PREFIX = "Interventie:";
        private readonly Regex PERIOD_STIPT_UUR_REGEX = new Regex("^([\\d\\.:,])*(\\s)*uur\\s(stipt)");
        private readonly Regex PERIOD_BEPAALD_UUR_REGEX = new Regex("^([\\d\\.:,])*\\suur");
        private readonly Regex PERIOD_VAN_TOT_REGEX = new Regex("^[\\d\\.:,]*-[\\d\\.:,]*\\s");

        private readonly IGraphServiceClient _client;
        private readonly CalendarConfiguration _calendarConfig;
        private readonly IEmployeeDataProvider _employeeDataProvider;
        private readonly ILogger _logger;

        public GraphApiCalendarService(IAuthenticationProvider authenticationProvider, IOptions<CalendarConfiguration> calendarConfiguration,
                                IEmployeeDataProvider employeeDataProvider, ILogger<GraphApiCalendarService> logger)
        {
            _client = new GraphServiceClient(authenticationProvider);
            _calendarConfig = calendarConfiguration.Value;
            _employeeDataProvider = employeeDataProvider;
            _logger = logger;

            if (_calendarConfig.PostponeWithYears == null)
                _calendarConfig.PostponeWithYears = 0;
        }

        public async Task<CalendarEvent> CreateEventForRequestAsync(CalendarEvent calendarEvent, Customer customer, Building building)
        {
            if (calendarEvent.VisitDate != null)
            {
                var msEvent = GenerateCreateRequestVisitEvent(calendarEvent, customer, building);

                try
                {
                    msEvent = await _client.Users[_calendarConfig.KlantenbezoekCalendarId].Calendar.Events.Request().AddAsync(msEvent);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Something went wrong while creating MS event for calendar event {1}.", calendarEvent.Id);
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

        public async Task<CalendarEvent> UpdateEventForRequestAsync(CalendarEvent calendarEvent, Customer customer, Building building, bool requiresReschedule)
        {
            if (calendarEvent.MsObjectId != null)
            {
                if (calendarEvent.VisitDate != null)
                {
                    var updatedMsEvent = requiresReschedule ? GenerateCreateRequestVisitEvent(calendarEvent, customer, building) : GenerateUpdateRequestVisitEvent(calendarEvent, customer, building);
                    try
                    {
                        updatedMsEvent = await _client.Users[_calendarConfig.KlantenbezoekCalendarId].Calendar.Events[calendarEvent.MsObjectId]
                                    .Request().UpdateAsync(updatedMsEvent);
                    }
                    catch (ServiceException e)
                    {
                        if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            _logger.LogWarning("Request to update calendar event {0}, but event doesn't exist anymore. Event will be recreated.", calendarEvent.Id);
                            return await CreateEventForRequestAsync(calendarEvent, customer, building);
                        }
                        else
                        {
                            throw e;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e, "Something went wrong while updating MS event {0} for calendar event {1}. Event will be decoupled.", calendarEvent.MsObjectId, calendarEvent.Id);
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
                        _logger.LogDebug("MS event of calendar event {0} has already been deleted.", calendarEvent.Id);
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

        public async Task<PlanningEvent> EnrichPlanningEventAsync(PlanningEvent planningEvent)
        {
            try
            {
                var calendarItem = await _client.Users[_calendarConfig.PlanningCalendarId].Calendar.Events[planningEvent.MsObjectId].Request().GetAsync();

                if (calendarItem != null)
                {
                    planningEvent.MsObjectId = calendarItem.Id;
                    planningEvent.Subject = calendarItem.Subject;
                }
                else
                {
                    _logger.LogWarning("No calendar item found for planning-event {0}. Has the item been deleted in the agenda?", planningEvent.Id);
                    planningEvent.IsNotAvailableInCalendar = true;
                }
            }
            catch (ServiceException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("No calendar item found for planning-event {0}. Has the item been deleted in the agenda?", planningEvent.Id);
                    planningEvent.IsNotAvailableInCalendar = true;
                }
                else
                {
                    throw e;
                }
            }

            var subject = planningEvent.Subject;
            if (planningEvent.Intervention != null)
               subject = subject.Substring(INTERVENTION_SUBJECT_PREFIX.Length).Trim();
            planningEvent = ParsePeriodMessage(planningEvent, subject);

            return planningEvent;
        }

        public async Task<PlanningEvent> CreateEventForPlanningAsync(PlanningEvent planningEvent)
        {
            if (planningEvent.Date != null)
            {
                var calendarItem = await GenerateCalendarItemAsync(planningEvent);
                try
                {
                    calendarItem = await _client.Users[_calendarConfig.PlanningCalendarId].Calendar.Events.Request().AddAsync(calendarItem);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Something went wrong while creating calendar item for planning event {0}.", planningEvent.Id);
                }

                planningEvent.MsObjectId = calendarItem.Id;
                planningEvent.Subject = calendarItem.Subject;

                _logger.LogDebug("Created planning event in calendar {0} with id {1}", _calendarConfig.PlanningCalendarId, calendarItem.Id);
            }
            else
            {
                _logger.LogWarning("Unable to create item for planning-event {0} in calendar. Planning-date is missing.", planningEvent.Id);
            }

            return planningEvent;
        }

        public async Task<PlanningEvent> UpdateEventForPlanningAsync(PlanningEvent planningEvent, bool requiresReschedule)
        {
            if (planningEvent.MsObjectId != null)
            {
                if (planningEvent.Date != null)
                {
                    // planningEvent = await EnrichPlanningEventAsync(planningEvent);
                    var updatedCalendarItem = await GenerateCalendarItemAsync(planningEvent);

                    try
                    {
                        updatedCalendarItem = await _client.Users[_calendarConfig.PlanningCalendarId].Calendar.Events[planningEvent.MsObjectId]
                                    .Request().UpdateAsync(updatedCalendarItem);
                        planningEvent.Subject = updatedCalendarItem.Subject;
                    }
                    catch (ServiceException e)
                    {
                        if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            _logger.LogWarning("Request to update calendar item {0} for planning-event {1}, but event doesn't exist anymore. Event will be recreated.",
                                                planningEvent.MsObjectId, planningEvent.Id);
                            return await CreateEventForPlanningAsync(planningEvent);
                        }
                        else
                        {
                            throw e;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e, "Something went wrong while updating calendar item {0} for planning event {1}. Event will be decoupled.",
                                            planningEvent.MsObjectId, planningEvent.Id);
                        return await DeleteEventForPlanningAsync(planningEvent);
                    }

                    _logger.LogDebug("Updated planning event in calendar {0} with id {1}", _calendarConfig.PlanningCalendarId, updatedCalendarItem.Id);
                }
                else
                {
                    _logger.LogWarning("Unable to update item for planning-event {0} in calendar. Planning date is missing.", planningEvent.Id);
                }
            }
            else
            {
                _logger.LogWarning("Cannot update calendar item for planning-event {0} since ms-object-id is not set.", planningEvent.Id);
            }

            return planningEvent;
        }

        public async Task<PlanningEvent> DeleteEventForPlanningAsync(PlanningEvent planningEvent)
        {
            if (planningEvent.MsObjectId != null)
            {
                try
                {
                    await _client.Users[_calendarConfig.PlanningCalendarId].Calendar.Events[planningEvent.MsObjectId].Request().DeleteAsync();
                    _logger.LogDebug("Delete planning-event in calendar {0} with id {1}", _calendarConfig.PlanningCalendarId, planningEvent.MsObjectId);
                }
                catch(Exception e) {
                    _logger.LogWarning(e, "Something went wrong while deleting calendar item {0} for planning-event {1}. Event will be decoupled.",
                                        planningEvent.MsObjectId, planningEvent.Id, e);
                }
                planningEvent.MsObjectId = null;
                planningEvent.Date = null;
                planningEvent.Subject = null;
            }
            else
            {
                _logger.LogWarning("Cannot delete calendar-item for planning-event {0} since ms-object-id is not set on the plannig-event.", planningEvent.Id);
            }

            return planningEvent;
        }

        public async Task<Order> CreateEventForPlanningAsync(Order order)
        {
            if (order.PlanningDate != null)
            {
                var calendarEvent = await GenerateCreatePlanningEventAsync(order);
                try
                {
                    calendarEvent = await _client.Users[_calendarConfig.PlanningCalendarId].Calendar.Events.Request().AddAsync(calendarEvent);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Something went wrong while creating planning event for order {0}.", order.Id);
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

        public async Task<Order> UpdateEventForPlanningAsync(Order order, bool requiresReschedule)
        {
            if (order.PlanningMsObjectId != null)
            {
                if (order.PlanningDate != null)
                {
                    var updatedEvent = requiresReschedule ? await GenerateCreatePlanningEventAsync(order) : await GenerateUpdatePlanningEventAsync(order);

                    try
                    {
                        updatedEvent = await _client.Users[_calendarConfig.PlanningCalendarId].Calendar.Events[order.PlanningMsObjectId]
                                    .Request().UpdateAsync(updatedEvent);
                    }
                    catch (ServiceException e)
                    {
                        if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            _logger.LogWarning("Request to update planning event {0} for order {1}, but event doesn't exist anymore. Event will be recreated.", order.PlanningMsObjectId, order.Id);
                            return await CreateEventForPlanningAsync(order);
                        }
                        else
                        {
                            throw e;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e, "Something went wrong while updating planning event {0} for order {1}. Event will be decoupled.", order.PlanningMsObjectId, order.Id);
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
                    _logger.LogWarning(e, "Something went wrong while deleting planning event {0} for order {1}. Event will be decoupled.", order.PlanningMsObjectId, order.Id, e);
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

        private Event GenerateCreateRequestVisitEvent(CalendarEvent calendarEvent, Customer customer, Building building)
        {
            var visitDate = (DateTime) calendarEvent.VisitDate;
            var year = visitDate.Year + (int) _calendarConfig.PostponeWithYears;
            var month = visitDate.Month;
            var day = visitDate.Day;

            var start = new DateTimeTimeZone() {
                DateTime = new DateTime(year, month, day, VISIT_START_TIME, 0, 0).ToString("o"),
                TimeZone = "Romance Standard Time"
            };
            var end = new DateTimeTimeZone() {
                DateTime = new DateTime(year, month, day, VISIT_START_TIME + 1, 0, 0).ToString("o"),
                TimeZone = "Romance Standard Time"
            };

            var subject = GenerateRequestVisitEventSubject(calendarEvent, customer, building);

            return new Event
            {
                Subject = subject,
                Start = start,
                End = end
            };
        }

        private Event GenerateUpdateRequestVisitEvent(CalendarEvent calendarEvent, Customer customer, Building building)
        {
            return new Event
            {
                Subject = GenerateRequestVisitEventSubject(calendarEvent, customer, building)
            };
        }

        private string GenerateRequestVisitEventSubject(CalendarEvent calendarEvent, Customer customer, Building building)
        {
            var period = FormatPeriodMessage(calendarEvent.Period, calendarEvent.FromHour, calendarEvent.UntilHour);

            var address = "";
            if (building != null)
                address = $"{building.PostalCode} {building.City}";
            else
                address = $"{customer.PostalCode} {customer.City}";

            return $"{period} {customer.Name} {address} ({calendarEvent.Request.Comment})";
        }

        private async Task<Event> GenerateCalendarItemAsync(PlanningEvent planningEvent)
        {

            var planningDate = (DateTime) planningEvent.Date;
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

            var subject = await GeneratePlanningEventSubjectAsync(planningEvent);

            return new Event
            {
                Subject = subject,
                Start = start,
                End = end
            };
        }
        private async Task<string> GeneratePlanningEventSubjectAsync(PlanningEvent planningEvent)
        {
            var period = FormatPeriodMessage(planningEvent.Period, planningEvent.FromHour, planningEvent.UntilHour);

            if (planningEvent.Intervention != null)
                return $"{INTERVENTION_SUBJECT_PREFIX} {period} {planningEvent.Intervention.CalendarSubject}";
            else
                return "";
        }

        private async Task<Event> GenerateCreatePlanningEventAsync(Order order)
        {
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

            var subject = await GeneratePlanningEventSubjectAsync(order);

            return new Event
            {
                Subject = subject,
                Start = start,
                End = end
            };
        }

        private async Task<Event> GenerateUpdatePlanningEventAsync(Order order)
        {
            return new Event
            {
                Subject = await GeneratePlanningEventSubjectAsync(order)
            };
        }

        private async Task<string> GeneratePlanningEventSubjectAsync(Order order)
        {
            var entity = order.Building != null ? (CustomerEntity) order.Building : order.Customer;
            var addressLines = string.Join(",", new string[3] { entity.Address1, entity.Address2, entity.Address3 }.Where(a => !String.IsNullOrEmpty(a)));
            var address = $"{entity.PostalCode} {entity.City} ({addressLines})";

            var subject = $"{order.Customer.Name} - {address} ** AD{order.RequestNumber}";

            if (order.MustBeDelivered)
            {
                subject = $"{subject} - Te leveren";
            }
            else
            {
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

                subject = $"{subject} - ({visitorInitials}) - {order.ScheduledHours}*{order.ScheduledNbOfPersons}";
            }

            return subject;
        }

        private async Task<string> GetSubjectAsync(string calendarId, string msObjectId)
        {
            try
            {
                var calendarEvent = await _client.Users[calendarId].Calendar.Events[msObjectId].Request().GetAsync();
                if (calendarEvent != null)
                    return calendarEvent.Subject;
                else
                    throw new EntityNotFoundException();
            }
            catch (ServiceException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new EntityNotFoundException();
                else
                    throw e;
            }
        }

        private PlanningEvent ParsePeriodMessage(PlanningEvent planningEvent, string subject)
        {
            if (subject != null)
            {
                if (subject.StartsWith("GD") || subject.StartsWith("VM") || subject.StartsWith("NM"))
                {
                    planningEvent.Period = subject.Substring(0, 2);
                    planningEvent.FromHour = null;
                    planningEvent.UntilHour = null;
                }
                else if (subject.StartsWith("vanaf"))
                {
                    planningEvent.Period = "vanaf";
                    planningEvent.FromHour = subject.Substring("vanaf".Length).Split("uur").FirstOrDefault().Trim();
                    planningEvent.UntilHour = null;
                }
                else if (subject.StartsWith("rond"))
                {
                    planningEvent.Period = "benaderend uur";
                    planningEvent.FromHour = subject.Substring("rond".Length).Split("uur").FirstOrDefault().Trim();
                    planningEvent.UntilHour = null;
                }
                else if (PERIOD_STIPT_UUR_REGEX.IsMatch(subject))
                {
                    planningEvent.Period = "stipt uur";
                    planningEvent.FromHour = subject.Split("uur").FirstOrDefault().Trim();
                    planningEvent.UntilHour = null;
                }
                else if (PERIOD_BEPAALD_UUR_REGEX.IsMatch(subject))
                {
                    planningEvent.Period = "bepaald uur";
                    planningEvent.FromHour = subject.Split("uur").FirstOrDefault().Trim();
                    planningEvent.UntilHour = null;
                }
                else if (PERIOD_VAN_TOT_REGEX.IsMatch(subject))
                {
                    planningEvent.Period = "van-tot";
                    var timeRangeSeparatorIndex = subject.IndexOf('-');
                    planningEvent.FromHour = subject.Substring(0, timeRangeSeparatorIndex).Trim();
                    var timeRangeEndIndex = subject.IndexOf(" ");
                    planningEvent.UntilHour = subject.Substring(timeRangeSeparatorIndex + 1, timeRangeEndIndex - timeRangeSeparatorIndex);
                }
                else
                {
                    _logger.LogWarning("Unable to parse period from subject {0} for planning-event {1} and calendar-item {2}", subject, planningEvent.Id, planningEvent.MsObjectId);
                    planningEvent.Period = null;
                    planningEvent.FromHour = null;
                    planningEvent.UntilHour = null;
                }
            }
            else
            {
                _logger.LogWarning("Visit doesn't have a subject. Unable to parse period for plannig-event {0} and calendar {1}", planningEvent.Id, planningEvent.MsObjectId);
                planningEvent.Period = null;
                planningEvent.FromHour = null;
                planningEvent.UntilHour = null;
            }

            return planningEvent;
        }

        private string FormatPeriodMessage(string period, string from, string until)
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
