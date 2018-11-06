using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Extensions;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProviders
{
    public class VisitDataProvider : IVisitDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public VisitDataProvider(CrmContext context, IMapper mapper, ILogger<VisitDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Visit> GetByIdAsync(int id, QuerySet query = null)
        {
            var visit = await FindByIdAsync(id, query);

            if (visit == null)
            {
                _logger.LogError($"No visit found with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Visit>(visit);
        }

        public async Task<Visit> GetByRequestIdAsync(int id)
        {
            var visit = await _context.Requests.Where(c => c.Id == id).Select(c => c.Visit).FirstOrDefaultAsync();

            if (visit == null)
            {
                _logger.LogError($"No visit found for request with id {id}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Visit>(visit);
        }

        public async Task<Visit> CreateAsync(Visit visit)
        {
            var visitRecord = _mapper.Map<DataProvider.Models.Visit>(visit);

            await CopyFieldsOfRequestAsync(visitRecord, visit.Request.Id);

            _context.Visits.Add(visitRecord);
            await _context.SaveChangesAsync();

            var savedVisit = _mapper.Map<Visit>(visitRecord);
            savedVisit.Period = visit.Period;
            savedVisit.FromHour = visit.FromHour;
            savedVisit.UntilHour = visit.UntilHour;

            return savedVisit;
        }

        public async Task<Visit> UpdateAsync(Visit visit)
        {
            var visitRecord = await FindByIdAsync(visit.Id);
            _mapper.Map(visit, visitRecord);

            await CopyFieldsOfRequestAsync(visitRecord, visit.Request.Id);

            _context.Visits.Update(visitRecord);
            await _context.SaveChangesAsync();

            var savedVisit = _mapper.Map<Visit>(visitRecord);
            savedVisit.Period = visit.Period;
            savedVisit.FromHour = visit.FromHour;
            savedVisit.UntilHour = visit.UntilHour;

            return savedVisit;
        }


        public async Task DeleteByIdAsync(int id)
        {
            var visit = await FindByIdAsync(id);

            if (visit != null)
            {
                _context.Visits.Remove(visit);
                await _context.SaveChangesAsync();
           }
        }

        private async Task<DataProvider.Models.Visit> FindByIdAsync(int? id, QuerySet query = null)
        {
            var source = _context.Visits.Where(c => c.Id == id);

            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }

        private async Task CopyFieldsOfRequestAsync(DataProvider.Models.Visit visitRecord, int requestId)
        {
            var requestRecord = await _context.Requests.Where(r => r.Id == requestId).FirstOrDefaultAsync();
            if (requestRecord != null)
            {
                visitRecord.CustomerId = requestRecord.CustomerId;
                visitRecord.RelativeContactId = requestRecord.RelativeContactId;
                visitRecord.RelativeBuildingId = requestRecord.RelativeBuildingId;
                visitRecord.Comment = requestRecord.Comment;
                visitRecord.EmbeddedCity = requestRecord.EmbeddedCity;
            }
        }
    }
}