using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProvider.Extensions
{
    public static class InvoicelineQueryExtensions
    {
        public static IQueryable<Invoiceline> Include(this IQueryable<Invoiceline> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Invoiceline, object>>>();

            selectors.Add("order", c => c.Order);
            selectors.Add("invoice", c => c.Invoice);
            selectors.Add("vat-rate", c => c.VatRate);

            return source.Include<Invoiceline>(querySet, selectors);
        }

        public static IQueryable<Invoiceline> Sort(this IQueryable<Invoiceline> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Invoiceline, object>>>();

            selectors.Add("amount", x => x.Amount);
            selectors.Add("sequence-number", x => x.SequenceNumber);

            return source.Sort<Invoiceline>(querySet, selectors);
        }
    }
}