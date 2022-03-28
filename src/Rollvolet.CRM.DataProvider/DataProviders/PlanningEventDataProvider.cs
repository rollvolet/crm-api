using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Extensions;
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
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }
    }
}