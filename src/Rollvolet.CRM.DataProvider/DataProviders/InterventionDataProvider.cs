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
    public class InterventionDataProvider : IInterventionDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public InterventionDataProvider(CrmContext context, IMapper mapper, ILogger<InterventionDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Paged<Intervention>> GetAllAsync(QuerySet query)
        {
            var source = _context.Interventions
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

            var interventions = await source.ForPage(query).ToListAsync();

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

        public async Task<Paged<Intervention>> GetAllByOrderIdAsync(int orderId, QuerySet query)
        {
            return await GetAllWhereAsync(r => r.OriginId == orderId, query);
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

        public async Task<Intervention> CreateAsync(Intervention intervention)
        {
            var interventionRecord = _mapper.Map<DataProvider.Models.Intervention>(intervention);
            _context.Interventions.Add(interventionRecord);
            await _context.SaveChangesAsync();

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

            var interventions = await source.ForPage(query).ToListAsync();

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
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }
    }
}