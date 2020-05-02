using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProvider.Extensions
{
    public static class PlanningEventQueryExtensions
    {
        public static IQueryable<PlanningEvent> Include(this IQueryable<PlanningEvent> source, QuerySet querySet)
        {
            if (querySet.Include.Fields.Contains("intervention.customer"))
                source = source.Include(x => x.Intervention).ThenInclude(x => x.Customer);
            if (querySet.Include.Fields.Contains("intervention.building"))
                source = source.Include(x => x.Intervention).ThenInclude(x => x.Building);
            if (querySet.Include.Fields.Contains("intervention.contact"))
                source = source.Include(x => x.Intervention).ThenInclude(x => x.Contact);

            var selectors = new Dictionary<string, Expression<Func<PlanningEvent, object>>>();

            selectors.Add("intervention", c => c.Intervention);

            // dummy entries for resources that are already included
            selectors.Add("intervention.customer", null);
            selectors.Add("intervention.building", null);
            selectors.Add("intervention.contact", null);

            return source.Include<PlanningEvent>(querySet, selectors);
        }

        public static IQueryable<PlanningEvent> Sort(this IQueryable<PlanningEvent> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<PlanningEvent, object>>>();

            selectors.Add("date", x => x.Date);

            return source.Sort<PlanningEvent>(querySet, selectors);
        }
    }
}