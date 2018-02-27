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
    public static class InvoiceQueryExtensions
    {
        public static IQueryable<Invoice> Filter(this IQueryable<Invoice> source, QuerySet querySet, CrmContext context)  
        {
            source = source.FilterCase(querySet, context);

            return source;
        }      

        public static IQueryable<Invoice> Include(this IQueryable<Invoice> source, QuerySet querySet)
        {
            if (querySet.Include.Fields.Contains("customer.honorific-prefix"))
                source = source.Include(x => x.Customer).ThenInclude(x => x.HonorificPrefix);

            var selectors = new Dictionary<string, Expression<Func<Invoice, object>>>();

            selectors.Add("customer", c => c.Customer);
            selectors.Add("order", c => c.Order);
            selectors.Add("vat-rate", c => c.VatRate);

            // The selectors below won't work since we're not able to define the relationship in CrmContext
            // They are manually mapped in the DataProvider
            // selectors.Add("building", c => c.Building);
            // selectors.Add("contact", c => c.Contact);

            return source.Include<Invoice>(querySet, selectors);
        }

        public static IQueryable<Invoice> Sort(this IQueryable<Invoice> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Invoice, object>>>();

            selectors.Add("invoice-date", x => x.InvoiceDate);
            selectors.Add("year", x => x.Year);
            selectors.Add("number", x => x.Number);
            selectors.Add("reference", x => x.Reference);
            selectors.Add("customer.name", x => x.CustomerName);
            selectors.Add("customer.street", x => x.CustomerAddress1);
            selectors.Add("customer.postal-code", x => x.CustomerPostalCode);
            selectors.Add("customer.city", x => x.CustomerCity);
            selectors.Add("updated", x => x.Updated);

            return source.Sort<Invoice>(querySet, selectors);
        }

    }
}