using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProvider.Extensions
{
    public static class InterventionQueryExtensions
    {
        public static IQueryable<Intervention> Filter(this IQueryable<Intervention> source, QuerySet querySet, CrmContext context)
        {
            if (querySet.Filter.Fields.ContainsKey("number"))
            {
                var filterValue = querySet.Filter.Fields["number"];
                int number;
                if (Int32.TryParse(filterValue, out number)) {
                    var predicate = PredicateBuilder.New<Intervention>(c => c.Id == number);
                    var i = 10;
                    while (i * number < 1000000) {
                        var from = i * number;
                        var to = i * (number + 1);
                        predicate.Or(c => c.Id >= from && c.Id <= to);
                        i = i * 10;
                    }
                    source = source.Where(predicate);
                }
                else
                {
                    throw new IllegalArgumentException("IllegalFilter", "Number filter must be a integer.");
                }
            }

            if (querySet.Filter.Fields.ContainsKey("invoice") && querySet.Filter.Fields["invoice"] == "false")
                source = source.Where(e => e.Invoice == null);

            source = source.FilterCase(querySet, context);

            return source;
        }

        public static IQueryable<Intervention> Include(this IQueryable<Intervention> source, QuerySet querySet)
        {
            if (querySet.Include.Fields.Contains("customer.honorific-prefix"))
                source = source.Include(x => x.Customer).ThenInclude(x => x.HonorificPrefix);
            if (querySet.Include.Fields.Contains("customer.language"))
                source = source.Include(x => x.Customer).ThenInclude(x => x.Language);
            if (querySet.Include.Fields.Contains("technicians"))
                source = source.Include(x => x.InterventionTechnicians).ThenInclude(x => x.Employee);

            var selectors = new Dictionary<string, Expression<Func<Intervention, object>>>();

            selectors.Add("customer", c => c.Customer);
            selectors.Add("way-of-entry", c => c.WayOfEntry);
            selectors.Add("invoice", x => x.Invoice);
            selectors.Add("origin", x => x.Origin);
            selectors.Add("follow-up-request", x => x.FollowUpRequest);

            // dummy entries for resources that are already included
            selectors.Add("customer.honorific-prefix", null);
            selectors.Add("customer.language", null);
            selectors.Add("technicians", null);

            // The selectors below won't work since we're not able to define the relationship in CrmContext
            // They are manually mapped in the DataProvider
            // selectors.Add("building", c => c.Building);
            // selectors.Add("contact", c => c.Contact);
            selectors.Add("building", null);
            selectors.Add("contact", null);

            return source.Include<Intervention>(querySet, selectors);
        }

        public static IQueryable<Intervention> Sort(this IQueryable<Intervention> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, IEnumerable<Expression<Func<Intervention, object>>>>();

            selectors.Add("number", new List<Expression<Func<Intervention, object>>> { x => x.Id });
            selectors.Add("date", new List<Expression<Func<Intervention, object>>> { x => x.Date, x => x.Id });
            selectors.Add("customer.name", new List<Expression<Func<Intervention, object>>> { x => x.Customer.Name });
            selectors.Add("customer.street", new List<Expression<Func<Intervention, object>>> { x => x.Customer.Address1 });
            selectors.Add("customer.postal-code", new List<Expression<Func<Intervention, object>>> { x => x.Customer.EmbeddedPostalCode });
            selectors.Add("customer.city", new List<Expression<Func<Intervention, object>>> { x => x.Customer.EmbeddedCity });

            return source.Sort<Intervention>(querySet, selectors);
        }

    }
}