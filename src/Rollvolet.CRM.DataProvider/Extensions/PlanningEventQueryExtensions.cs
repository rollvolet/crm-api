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

            var selectors = new Dictionary<string, Expression<Func<PlanningEvent, object>>>();

            selectors.Add("intervention", c => c.Intervention);

            // dummy entries for resources that are already included
            selectors.Add("intervention.customer", null);

            // The selectors below won't work since we're not able to define the relationship in CrmContext
            // They are manually mapped in the DataProvider
            // selectors.Add("building", c => c.Building);
            // selectors.Add("contact", c => c.Contact);
            selectors.Add("intervention.building", null);

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