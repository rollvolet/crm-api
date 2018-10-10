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
    public static class InvoiceSupplementQueryExtensions
    {
        public static IQueryable<InvoiceSupplement> Filter(this IQueryable<InvoiceSupplement> source, QuerySet querySet)
        {
            return source;
        }

        public static IQueryable<InvoiceSupplement> Include(this IQueryable<InvoiceSupplement> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<InvoiceSupplement, object>>>();

            selectors.Add("invoice", c => c.Invoice);

            return source.Include<InvoiceSupplement>(querySet, selectors);
        }

        public static IQueryable<InvoiceSupplement> Sort(this IQueryable<InvoiceSupplement> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<InvoiceSupplement, object>>>();

            selectors.Add("amount", x => x.Amount);

            return source.Sort<InvoiceSupplement>(querySet, selectors);
        }

    }
}