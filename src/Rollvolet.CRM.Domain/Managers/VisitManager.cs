using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
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
        private readonly ILogger _logger;

        public VisitManager(IVisitDataProvider visitDataProvider, IRequestDataProvider requestDataProvider, ILogger<VisitManager> logger)
        {
            _visitDataProvider = visitDataProvider;
            _requestDataProvider = requestDataProvider;
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
            if (visit.Customer != null)
            {
                var message = "Customer cannot be added to a visit on creation.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }

            await EmbedRelations(visit);

            // TODO create entry in Klantenbezoekagenda

            return await _visitDataProvider.CreateAsync(visit);
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

            await EmbedRelations(visit, existingVisit);

            // TODO update entry in Klantenbezoekagenda

            return await _visitDataProvider.UpdateAsync(visit);
        }

        public async Task DeleteAsync(int id)
        {
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
    }
}