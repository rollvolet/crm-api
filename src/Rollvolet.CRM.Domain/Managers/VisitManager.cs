using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
        private readonly Regex periodStiptUur = new Regex("^([\\d\\.:,])*(\\s)*uur\\s(stipt)");
        private readonly Regex periodBepaaldUur = new Regex("^([\\d\\.:,])*\\suur");
        private readonly Regex periodVanTot = new Regex("^[\\d\\.:,]*-[\\d\\.:,]*\\s");

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
            var visit = await _visitDataProvider.GetByIdAsync(id, query);

            if (visit.MsObjectId != null)
                visit = await SyncSubjectAndPeriod(visit);

            return visit;
        }

        public async Task<Visit> GetByRequestIdAsync(int id)
        {
            var visit = await _visitDataProvider.GetByRequestIdAsync(id);

            if (visit.MsObjectId != null)
                visit = await SyncSubjectAndPeriod(visit);

            return visit;
        }

        public async Task<Visit> CreateAsync(Visit visit)
        {
            // Validations
            if (visit.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Visit cannot have an id on create.");
            if (visit.VisitDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Visit date is required on visit creation.");
            if (visit.Period == null)
                throw new IllegalArgumentException("IllegalAttribute", "Period is required on visit creation.");
            if (visit.RequiresFromHour && visit.FromHour == null)
                throw new IllegalArgumentException("IllegalAttribute", $"FromHour is required on visit for period {visit.Period}.");
            if (visit.RequiresUntilHour && visit.UntilHour == null)
                throw new IllegalArgumentException("IllegalAttribute", $"UntilHour is required on visit for period {visit.Period}.");
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

            try
            {
                var customerEntity = await GetRelatedCustomerEntity(visit);
                visit = await _graphApiService.CreateCalendarEventForVisit(visit, customerEntity);

                return await _visitDataProvider.UpdateAsync(visit);
            }
            catch (Exception e)
            {
                await _visitDataProvider.DeleteByIdAsync(visit.Id);
                throw e;
            }
        }

        public async Task<Visit> UpdateAsync(Visit visit)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer", "request" };
            var existingVisit = await _visitDataProvider.GetByIdAsync(visit.Id, query);

            if (visit.IsMasteredByAccess)
                throw new IllegalArgumentException("IllegalAttribute", $"Visit {visit.Id} cannot be updated since it's mastered by Access.");
            if (visit.Id != existingVisit.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Visit id cannot be updated.");
            if (visit.VisitDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Visit date is required on visit.");
            if (visit.Period == null)
                throw new IllegalArgumentException("IllegalAttribute", "Period is required on visit.");
            if (visit.RequiresFromHour && visit.FromHour == null)
                throw new IllegalArgumentException("IllegalAttribute", $"FromHour is required on visit for period {visit.Period}.");
            if (visit.RequiresUntilHour && visit.UntilHour == null)
                throw new IllegalArgumentException("IllegalAttribute", $"UntilHour is required on visit for period {visit.Period}.");
            if (visit.Customer != null)
            {
                var message = "Customer cannot be set on a visit.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }
            if (visit.CalendarId != existingVisit.CalendarId || visit.MsObjectId != existingVisit.MsObjectId || visit.CalendarSubject != existingVisit.CalendarSubject)
                throw new IllegalArgumentException("IllegalAttribute", "Calendar properties cannot be updated.");

            await EmbedRelations(visit, existingVisit);

            if(RequiresCalendarEventUpdate(existingVisit, visit))
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

        private async Task<Visit> SyncSubjectAndPeriod(Visit visit)
        {
            var subject = await _graphApiService.GetSubject(visit.MsObjectId);

            if (visit.CalendarSubject != subject)
            {
                visit.CalendarSubject = subject;
                visit = await _visitDataProvider.UpdateAsync(visit);
            }

            ParsePeriodFromSubject(visit);

            return visit;
        }

        private void ParsePeriodFromSubject(Visit visit)
        {
            var subject = visit.CalendarSubject;

            if (subject.StartsWith("GD") || subject.StartsWith("VM") || subject.StartsWith("NM"))
            {
                visit.Period = subject.Substring(0, 2);
                visit.FromHour = null;
                visit.UntilHour = null;
            }
            else if (subject.StartsWith("vanaf"))
            {
                visit.Period = "vanaf";
                visit.FromHour = subject.Substring("vanaf".Length).Split("uur").FirstOrDefault().Trim();
                visit.UntilHour = null;
            }
            else if (subject.StartsWith("rond"))
            {
                visit.Period = "benaderend uur";
                visit.FromHour = subject.Substring("rond".Length).Split("uur").FirstOrDefault().Trim();
                visit.UntilHour = null;
            }
            else if (periodStiptUur.IsMatch(subject))
            {
                visit.Period = "stipt uur";
                visit.FromHour = subject.Split("uur").FirstOrDefault().Trim();
                visit.UntilHour = null;
            }
            else if (periodBepaaldUur.IsMatch(subject))
            {
                visit.Period = "bepaald uur";
                visit.FromHour = subject.Split("uur").FirstOrDefault().Trim();
                visit.UntilHour = null;
            }
            else if (periodVanTot.IsMatch(subject))
            {
                visit.Period = "van-tot";
                var timeRangeSeparatorIndex = subject.IndexOf('-');
                visit.FromHour = subject.Substring(0, timeRangeSeparatorIndex).Trim();
                var timeRangeEndIndex = subject.IndexOf(" ");
                visit.UntilHour = subject.Substring(timeRangeSeparatorIndex + 1, timeRangeEndIndex - timeRangeSeparatorIndex);
            }
            else
            {
                _logger.LogWarning("Unable to parse period from subject {0} for visit {1} and event {2}", subject, visit.Id, visit.MsObjectId);
                visit.Period = null;
                visit.FromHour = null;
                visit.UntilHour = null;
            }
        }

        private bool RequiresCalendarEventUpdate(Visit existingVisit, Visit visit)
        {
            ParsePeriodFromSubject(existingVisit);

            return visit.VisitDate != existingVisit.VisitDate
                || visit.Period != existingVisit.Period
                || visit.FromHour != existingVisit.FromHour
                || visit.UntilHour != existingVisit.UntilHour
                || visit.Request.Comment != existingVisit.Comment;
        }
    }
}