using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProvider.Extensions
{
    public static class WorkingHourQueryExtensions
    {
        public static IQueryable<WorkingHour> Filter(this IQueryable<WorkingHour> source, QuerySet querySet)
        {
            return source;
        }

        public static IQueryable<WorkingHour> Include(this IQueryable<WorkingHour> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<WorkingHour, object>>>();

            selectors.Add("employee", c => c.Employee);
            selectors.Add("invoice", c => c.Invoice);

            return source.Include<WorkingHour>(querySet, selectors);
        }

        public static IQueryable<WorkingHour> Sort(this IQueryable<WorkingHour> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<WorkingHour, object>>>();

            selectors.Add("date", x => x.Date);
            selectors.Add("employee", x => x.Employee.LastName);

            return source.Sort<WorkingHour>(querySet, selectors);
        }

    }
}