using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.DataProvider.Extensions;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Exceptions;
using System.Linq.Expressions;
using System;

namespace Rollvolet.CRM.DataProviders
{
    public class InterventionDataProvider : CaseRelatedDataProvider<DataProvider.Models.Intervention>, IInterventionDataProvider
    {
        public InterventionDataProvider(CrmContext context, IMapper mapper, ILogger<InterventionDataProvider> logger) : base(context, mapper, logger)
        {

        }

        public async Task<Paged<Intervention>> GetAllAsync(QuerySet query)
        {
            var source = _context.Interventions
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

            // EF Core doesn't support relationships with a derived type so we have to embed the related resource manually
            var interventions = QueryListWithManualInclude(source, query);

            var mappedInterventions = _mapper.Map<IEnumerable<Intervention>>(interventions);

            var count = await source.CountAsync();

            return new Paged<Intervention>() {
                Items = mappedInterventions,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Intervention> GetByIdAsync(int id, QuerySet query = null)
        {
            var intervention = await FindByIdAsync(id, query);

            if (intervention == null)
            {
                _logger.LogError($"No intervention found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Intervention>(intervention);
        }

        public async Task<Paged<Intervention>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            return await GetAllWhereAsync(r => r.CustomerId == customerId, query);
        }

        public async Task<Paged<Intervention>> GetAllByRelativeContactIdAsync(int customerId, int relativeContactId, QuerySet query)
        {
            return await GetAllWhereAsync(r => r.CustomerId == customerId && r.RelativeContactId == relativeContactId, query);
        }

        public async Task<Paged<Intervention>> GetAllByRelativeBuildingIdAsync(int customerId, int relativeBuildingId, QuerySet query)
        {
            return await GetAllWhereAsync(r => r.CustomerId == customerId && r.RelativeBuildingId == relativeBuildingId, query);
        }

        public async Task<Intervention> GetByInvoiceIdAsync(int invoiceId)
        {
            var intervention = await _context.Invoices.Where(r => r.Id == invoiceId)
                                    .Select(r => r.Intervention).FirstOrDefaultAsync();

            if (intervention == null)
            {
                _logger.LogError($"No intervention found for invoice id {invoiceId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Intervention>(intervention);
        }

        public async Task<Intervention> GetByFollowUpRequestIdAsync(int requestId)
        {
            var intervention = await _context.Requests.Where(r => r.Id == requestId)
                                    .Select(r => r.Origin).FirstOrDefaultAsync();

            if (intervention == null)
            {
                _logger.LogError($"No intervention found for follow up request id {requestId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Intervention>(intervention);
        }

        public async Task<Intervention> GetByPlanningEventIdAsync(int planningEventId)
        {
            var intervention = await _context.PlanningEvents.Where(r => r.Id == planningEventId)
                                    .Select(r => r.Intervention).FirstOrDefaultAsync();

            if (intervention == null)
            {
                _logger.LogError($"No intervention found for planning event id {planningEventId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Intervention>(intervention);
        }

        public async Task<Intervention> CreateAsync(Intervention intervention)
        {
            var interventionRecord = _mapper.Map<DataProvider.Models.Intervention>(intervention);
            _context.Interventions.Add(interventionRecord);
            await _context.SaveChangesAsync();

            var planningEventRecord = await CreatePlanningEventAsync(interventionRecord);

            return _mapper.Map<Intervention>(interventionRecord);
        }

        public async Task<Intervention> UpdateAsync(Intervention intervention)
        {
            var interventionRecord = await FindByIdAsync(intervention.Id);
            _mapper.Map(intervention, interventionRecord);

            _context.Interventions.Update(interventionRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Intervention>(interventionRecord);
        }

        public async Task<Intervention> UpdateContactAndBuildingAsync(int id, int? relativeContactId, int? relativeBuildingId)
        {
            var interventionRecord = await FindByIdAsync(id);
            interventionRecord.RelativeContactId = relativeContactId;
            interventionRecord.RelativeBuildingId = relativeBuildingId;

            _context.Interventions.Update(interventionRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Intervention>(interventionRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var intervention = await FindByIdAsync(id);

            if (intervention != null)
            {
                var planningEvent = await _context.PlanningEvents.Where(c => c.InterventionId == id).FirstOrDefaultAsync();

                if (planningEvent != null)
                    _context.PlanningEvents.Remove(planningEvent);

                _context.Interventions.Remove(intervention);
                await _context.SaveChangesAsync();
           }
        }

        private async Task<Paged<Intervention>> GetAllWhereAsync(Expression<Func<DataProvider.Models.Intervention, bool>> where, QuerySet query)
        {
            var source = _context.Interventions
                            .Where(where)
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

            var interventions = QueryListWithManualInclude(source, query);

            var mappedInterventions = _mapper.Map<IEnumerable<Intervention>>(interventions);

            var count = await source.CountAsync();

            return new Paged<Intervention>() {
                Items = mappedInterventions,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        private async Task<DataProvider.Models.Intervention> FindByIdAsync(int id, QuerySet query = null)
        {
            return await FindWhereAsync(c => c.Id == id, query);
        }

        private async Task<DataProvider.Models.Intervention> FindWhereAsync(Expression<Func<DataProvider.Models.Intervention, bool>> where, QuerySet query = null)
        {
            var source = (IQueryable<DataProvider.Models.Intervention>) _context.Interventions.Where(where);

            if (query != null)
            {
                source = source.Include(query);
                // EF Core doesn't support relationships with a derived type so we have to embed the related resource manually
                return await QueryWithManualIncludeAsync(source, query);
            }
            else
            {
                return await source.FirstOrDefaultAsync();
            }
        }

        private async Task<DataProvider.Models.PlanningEvent> CreatePlanningEventAsync(DataProvider.Models.Intervention interventionRecord)
        {
            var planningEvent = new DataProvider.Models.PlanningEvent {
                InterventionId = interventionRecord.Id
            };

            _context.PlanningEvents.Add(planningEvent);
            await _context.SaveChangesAsync();

            return planningEvent;
        }
    }
}