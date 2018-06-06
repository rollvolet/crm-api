using System;
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
    public class VisitManager : IVisitManager
    {
        private readonly IVisitDataProvider _visitDataProvider;
        private readonly IRequestDataProvider _requestDataProvider;
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly IGraphApiService _graphApiService;
        private readonly ILogger _logger;

        public VisitManager(IVisitDataProvider visitDataProvider, IRequestDataProvider requestDataProvider, ICustomerDataProvider customerDataProvider,
                            IGraphApiService graphApiService, ILogger<VisitManager> logger)
        {
            _visitDataProvider = visitDataProvider;
            _requestDataProvider = requestDataProvider;
            _customerDataProvider = customerDataProvider;
            _graphApiService = graphApiService;
            _logger = logger;
        }

        public async Task<Visit> GetByIdAsync(int id, QuerySet query = null)
        {
            return await _visitDataProvider.GetByIdAsync(id, query);
        }

        public async Task<Visit> GetByRequestIdAsync(int id)
        {
            return await _visitDataProvider.GetByRequestIdAsync(id);
        }

        public async Task<Visit> CreateAsync(Visit visit)
        {
            // Validations
            if (visit.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Visit cannot have an id on create.");
            if (visit.VisitDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Visit date is required on visit creation.");
            if (visit.Request == null)
                throw new IllegalArgumentException("IllegalAttribute", "Request is required on visit creation.");
            if (visit.CalendarId != null || visit.MsObjectId != null || visit.CalendarId != null)
                throw new IllegalArgumentException("IllegalAttribute", "Calendar properties cannot be set.");
            if (visit.Customer != null)
            {
                var message = "Customer cannot be added to a visit on creation.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }

            await EmbedRelations(visit);

            visit = await _visitDataProvider.CreateAsync(visit);

            var customerEntity = await GetRelatedCustomerEntity(visit);
            visit = await _graphApiService.CreateCalendarEventForVisit(visit, customerEntity);

            return await _visitDataProvider.UpdateAsync(visit);
        }

        public async Task<Visit> UpdateAsync(Visit visit)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer", "request" };
            var existingVisit = await _visitDataProvider.GetByIdAsync(visit.Id, query);

            if (visit.Id != existingVisit.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Visit id cannot be updated.");
            if (visit.VisitDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Visit date is required on visit creation.");
            if (visit.Customer != null)
            {
                var message = "Customer cannot be set on a visit.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }
            if (visit.CalendarId != existingVisit.CalendarId || visit.MsObjectId != existingVisit.MsObjectId || visit.CalendarSubject != existingVisit.CalendarSubject)
                throw new IllegalArgumentException("IllegalAttribute", "Calendar properties cannot be updated.");

            await EmbedRelations(visit, existingVisit);

            if (visit.VisitDate != existingVisit.VisitDate || visit.Request.Comment != existingVisit.Comment)
            {
                var customerEntity = await GetRelatedCustomerEntity(visit);
                visit = await _graphApiService.UpdateCalendarEventForVisit(visit, customerEntity);
            }

            return await _visitDataProvider.UpdateAsync(visit);
        }

        public async Task DeleteAsync(int id)
        {
            var visit = await _visitDataProvider.GetByIdAsync(id);

            await _graphApiService.DeleteCalendarEventForVisit(visit);
            await _visitDataProvider.DeleteByIdAsync(id);
        }

        // Embed relations in visit: reuse old relation if there is one and it hasn't changed
        private async Task EmbedRelations(Visit visit, Visit oldVisit = null)
        {
            try {
                // Request cannot be updated. Take request of oldVisit on update.
                if (oldVisit != null)
                    visit.Request = oldVisit.Request;
                else
                    visit.Request = await _requestDataProvider.GetByIdAsync(visit.Request.Id);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }

        private async Task<CustomerEntity> GetRelatedCustomerEntity(Visit visit)
        {
            // TODO must this be the building if there is one?
            return await _customerDataProvider.GetByRequestIdAsync(visit.Request.Id);
        }
    }
}