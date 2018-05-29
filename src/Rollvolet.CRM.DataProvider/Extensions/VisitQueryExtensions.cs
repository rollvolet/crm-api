using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProvider.Extensions
{
    public static class VisitQueryExtensions
    {
        public static IQueryable<Visit> Include(this IQueryable<Visit> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Visit, object>>>();

            selectors.Add("request", c => c.Request);
            selectors.Add("customer", c => c.Customer);

            return source.Include<Visit>(querySet, selectors);
        }

        public static IQueryable<Visit> Sort(this IQueryable<Visit> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Visit, object>>>();

            selectors.Add("visit-date", x => x.VisitDate);

            return source.Sort<Visit>(querySet, selectors);
        }
    }
}