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
    public class PlanningEventManager : IPlanningEventManager
    {
        private readonly IPlanningEventDataProvider  _planningEventDataProvider;
        private readonly IInterventionDataProvider _interventionDataProvider;
        private readonly IEmployeeDataProvider _employeeDataProvider;
        private readonly IGraphApiCalendarService _calendarService;
        private readonly ILogger _logger;

        public PlanningEventManager(IPlanningEventDataProvider planningEventDataProvider, IInterventionDataProvider interventionDataProvider,
                                IEmployeeDataProvider employeeDataProvider, IGraphApiCalendarService calendarService, ILogger<InterventionManager> logger)
        {
            _planningEventDataProvider = planningEventDataProvider;
            _interventionDataProvider = interventionDataProvider;
            _employeeDataProvider = employeeDataProvider;
            _calendarService = calendarService;
            _logger = logger;
        }

        public async Task<PlanningEvent> GetByIdAsync(int id, QuerySet query = null)
        {
            var planningEvent = await _planningEventDataProvider.GetByIdAsync(id, query);

            if (planningEvent.MsObjectId != null)
                planningEvent = await _calendarService.EnrichPlanningEventAsync(planningEvent);

            return planningEvent;
        }

        public async Task<PlanningEvent> GetByInterventionIdAsync(int planningEventId)
        {
            var planningEvent = await _planningEventDataProvider.GetByInterventionIdAsync(planningEventId);

            if (planningEvent.MsObjectId != null)
                planningEvent = await _calendarService.EnrichPlanningEventAsync(planningEvent);

            return planningEvent;
        }

        public async Task<PlanningEvent> UpdateAsync(PlanningEvent planningEvent)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "intervention", "intervention.customer", "intervention.building" };
            var existingPlanningEvent = await _planningEventDataProvider.GetByIdAsync(planningEvent.Id, query);

            if (planningEvent.Id != existingPlanningEvent.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Planning-event id cannot be updated.");

            await EmbedRelations(planningEvent, existingPlanningEvent);

            if (planningEvent.Date != null)
            {
                if (existingPlanningEvent.MsObjectId == null)
                {
                    planningEvent = await _calendarService.CreateEventForPlanningAsync(planningEvent);
                }
                else
                {
                    var requiresReschedule = existingPlanningEvent.Date != planningEvent.Date;
                    planningEvent = await _calendarService.UpdateEventForPlanningAsync(planningEvent, requiresReschedule);
                }
            }
            else if (planningEvent.Date == null && existingPlanningEvent.MsObjectId != null)
            {
                planningEvent = await _calendarService.DeleteEventForPlanningAsync(planningEvent);
            }

            var savedPlanningEvent = await _planningEventDataProvider.UpdateAsync(planningEvent);

            // Copy properties from incoming domain resource to outgoing domain resource
            savedPlanningEvent.Period = planningEvent.Period;
            savedPlanningEvent.FromHour = planningEvent.FromHour;
            savedPlanningEvent.UntilHour = planningEvent.UntilHour;

            return savedPlanningEvent;
        }

        private async Task EmbedRelations(PlanningEvent planningEvent, PlanningEvent oldPlanningEvent = null)
        {
            try {
                // Intervention cannot be updated. Take intervention of oldPlanningEvent on update.
                if (oldPlanningEvent != null)
                    planningEvent.Intervention = oldPlanningEvent.Intervention;
                else
                    planningEvent.Intervention = null;

                // Order cannot be updated. Take order of oldPlanningEvent on update.
                if (oldPlanningEvent != null)
                    planningEvent.Order = oldPlanningEvent.Order;
                else
                    planningEvent.Order = null;
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }
    }
}