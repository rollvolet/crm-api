using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Extensions;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProviders
{
    public class PlanningEventDataProvider : IPlanningEventDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PlanningEventDataProvider(CrmContext context, IMapper mapper, ILogger<PlanningEventDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Domain.Models.PlanningEvent> GetByIdAsync(int id, QuerySet query = null)
        {
            var planningEvent = await FindByIdAsync(id, query);

            if (planningEvent == null)
            {
                _logger.LogError($"No planning event found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Domain.Models.PlanningEvent>(planningEvent);
        }

        public async Task<Domain.Models.PlanningEvent> GetByInterventionIdAsync(int id)
        {
            var planningEvent = await FindWhereAsync(c => c.InterventionId == id);

            if (planningEvent == null)
            {
                _logger.LogError($"No planning-event found for intervention with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Domain.Models.PlanningEvent>(planningEvent);
        }

        // Creation of a Planning is handled by the InterventionDataProvider/OrderDataProvider
        // PlanningEvent can only be created on creation of a new intervention/order

        public async Task<Domain.Models.PlanningEvent> UpdateAsync(Domain.Models.PlanningEvent planningEvent)
        {
            var planningEventRecord = await FindByIdAsync(planningEvent.Id);
            _mapper.Map(planningEvent, planningEventRecord);

            _context.PlanningEvents.Update(planningEventRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Domain.Models.PlanningEvent>(planningEventRecord);
        }

        // Deletion of a PlanningEvent is handled by the InterventionDataProvider/OrderDataProvider
        // Planning event can only be deleted on deletion of the corresponding intervention/order

        private async Task<DataProvider.Models.PlanningEvent> FindByIdAsync(int id, QuerySet query = null)
        {
            return await FindWhereAsync(c => c.Id == id, query);
        }

        private async Task<DataProvider.Models.PlanningEvent> FindWhereAsync(Expression<Func<DataProvider.Models.PlanningEvent, bool>> where, QuerySet query = null)
        {
            // Intervention and order are always required in the business logic of planning events. Include those by default
            var source = (IQueryable<DataProvider.Models.PlanningEvent>) _context.PlanningEvents.Where(where)
                                                                                                .Include(x => x.Intervention);

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

        // TODO similar code is duplicated in the CaseDataProvider
        private async Task<DataProvider.Models.PlanningEvent> QueryWithManualIncludeAsync(IQueryable<DataProvider.Models.PlanningEvent> source, QuerySet query)
        {
            if (query.Include.Fields.Contains("intervention.building")  || query.Include.Fields.Contains("intervention.contact"))
            {
                var joinedSource = JoinBuildingAndContact(source);
                var triplet = await joinedSource.FirstOrDefaultAsync();

                if (triplet != null)
                    return EmbedBuildingAndContact(triplet);
                else
                    return default(DataProvider.Models.PlanningEvent);
            }
            else
            {
                return await source.FirstOrDefaultAsync();
            }
        }

        // TODO this method is duplicated in CaseDataProvider
        private IQueryable<CaseTriplet<DataProvider.Models.PlanningEvent>> JoinBuildingAndContact(IQueryable<DataProvider.Models.PlanningEvent> source)
        {
            return source.GroupJoin(
                    _context.Buildings.Include(b => b.HonorificPrefix),
                    s => new { CustomerId = s.Intervention.CustomerId, Number = s.Intervention.RelativeBuildingId },
                    b => new { CustomerId = (int?) b.CustomerId, Number = (int?) b.Number },
                    (s, b) => new { Source = s, Buildings = b }
                ).SelectMany(
                    t => t.Buildings.DefaultIfEmpty(),
                    (t, b) => new { Source = t.Source, Building = b }
                ).GroupJoin(
                    _context.Contacts.Include(c => c.HonorificPrefix),
                    t => new { CustomerId = t.Source.Intervention.CustomerId, Number = t.Source.Intervention.RelativeContactId },
                    c => new { CustomerId = (int?) c.CustomerId, Number = (int?) c.Number },
                    (t, c) => new { Source = t.Source, Building = t.Building, Contacts = c }
                ).SelectMany(
                    u => u.Contacts.DefaultIfEmpty(),
                    (u, c) => new CaseTriplet<DataProvider.Models.PlanningEvent> { Source = u.Source, Building = u.Building, Contact = c}
                );
        }

        private DataProvider.Models.PlanningEvent EmbedBuildingAndContact(CaseTriplet<DataProvider.Models.PlanningEvent> triplet)
        {
            var item = triplet.Source;

            if (item != null) {
                item.Intervention.Building = triplet.Building;
                item.Intervention.Contact = triplet.Contact;
            }

            return item;
        }

        private IEnumerable<DataProvider.Models.PlanningEvent> EmbedBuildingAndContact(IEnumerable<CaseTriplet<DataProvider.Models.PlanningEvent>> triplets)
        {
            var items = new List<DataProvider.Models.PlanningEvent>();

            foreach(var triplet in triplets)
            {
                var item = triplet.Source;
                item.Intervention.Building = triplet.Building;
                item.Intervention.Contact = triplet.Contact;
                items.Add(item);
            }

            return items;
        }
    }
}