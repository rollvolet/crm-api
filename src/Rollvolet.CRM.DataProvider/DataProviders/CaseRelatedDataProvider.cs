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
using LinqKit;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.DataProvider.Models.Interfaces;

namespace Rollvolet.CRM.DataProviders
{
    public class CaseRelatedDataProvider<T> where T : ICaseRelated
    {
        protected readonly CrmContext _context;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;

        public CaseRelatedDataProvider(CrmContext context, IMapper mapper, ILogger<CaseRelatedDataProvider<T>> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        protected IEnumerable<T> QueryListWithManualInclude(IQueryable<T> source, QuerySet query)
        {
            if (query.Include.Fields.Contains("building")  || query.Include.Fields.Contains("contact"))
            {
                var joinedSource = JoinBuildingAndContact(source);
                var triplets = joinedSource.ForPage(query).AsEnumerable();
                return EmbedBuildingAndContact(triplets);
            }
            else
            {
                return source.ForPage(query).AsEnumerable();
            }
        }
        protected async Task<T> QueryWithManualIncludeAsync(IQueryable<T> source, QuerySet query)
        {
            if (query.Include.Fields.Contains("building")  || query.Include.Fields.Contains("contact"))
            {
                var joinedSource = JoinBuildingAndContact(source);
                var triplet = await joinedSource.FirstOrDefaultAsync();

                if (triplet != null)
                    return EmbedBuildingAndContact(triplet);
                else
                    return default(T);
            }
            else
            {
                return await source.FirstOrDefaultAsync();
            }
        }

        private IQueryable<CaseTriplet<T>> JoinBuildingAndContact(IQueryable<T> source)
        {
            return source.GroupJoin(
                    _context.Buildings.Include(b => b.HonorificPrefix),
                    s => new { CustomerId = s.CustomerId, Number = s.RelativeBuildingId },
                    b => new { CustomerId = (int?) b.CustomerId, Number = (int?) b.Number },
                    (s, b) => new { Source = s, Buildings = b }
                ).SelectMany(
                    t => t.Buildings.DefaultIfEmpty(),
                    (t, b) => new { Source = t.Source, Building = b }
                ).GroupJoin(
                    _context.Contacts.Include(c => c.HonorificPrefix),
                    t => new { CustomerId = t.Source.CustomerId, Number = t.Source.RelativeContactId },
                    c => new { CustomerId = (int?) c.CustomerId, Number = (int?) c.Number },
                    (t, c) => new { Source = t.Source, Building = t.Building, Contacts = c }
                ).SelectMany(
                    u => u.Contacts.DefaultIfEmpty(),
                    (u, c) => new CaseTriplet<T> { Source = u.Source, Building = u.Building, Contact = c}
                );
        }

        private T EmbedBuildingAndContact(CaseTriplet<T> triplet)
        {
            var item = triplet.Source;

            if (item != null) {
                item.Building = triplet.Building;
                item.Contact = triplet.Contact;
            }

            return item;
        }

        private IEnumerable<T> EmbedBuildingAndContact(IEnumerable<CaseTriplet<T>> triplets)
        {
            var items = new List<T>();

            foreach(var triplet in triplets)
            {
                var item = triplet.Source;
                item.Building = triplet.Building;
                item.Contact = triplet.Contact;
                items.Add(item);
            }

            return items;
        }
    }
}