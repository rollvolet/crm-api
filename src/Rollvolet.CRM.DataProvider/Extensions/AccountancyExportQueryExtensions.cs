using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProvider.Extensions
{
    public static class AccountancyExportQueryExtensions
    {
        public static IQueryable<AccountancyExport> Filter(this IQueryable<AccountancyExport> source, QuerySet querySet)
        {
            return source;
        }

        public static IQueryable<AccountancyExport> Sort(this IQueryable<AccountancyExport> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<AccountancyExport, object>>>();

            selectors.Add("date", x => x.Date);
            selectors.Add("from-date", x => x.FromDate);
            selectors.Add("until-date", x => x.UntilDate);

            return source.Sort<AccountancyExport>(querySet, selectors);
        }

    }
}