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
    public static class RequestQueryExtensions
    {
        public static IQueryable<Request> Filter(this IQueryable<Request> source, QuerySet querySet, CrmContext context)  
        {
            if (querySet.Filter.Fields.ContainsKey("number"))
            {
                var filterValue = querySet.Filter.Fields["number"];
                int number;
                if (Int32.TryParse(filterValue, out number)) {
                    var predicate = PredicateBuilder.New<Request>(c => c.Id == number);
                    var i = 10;
                    while (i * number < 1000000) {
                        var from = i * number;
                        var to = i * (number + 1);
                        predicate.Or(c => c.Id >= from && c.Id <= to);
                        i = i * 10;
                    }
                    source = source.Where(predicate);
                }
                // TODO throw exception about invalid number
            }

            source = source.FilterCase(querySet, context);

            // TODO filter met / zonder offerte

            return source;
        }      

        public static IQueryable<Request> Include(this IQueryable<Request> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Request, object>>>();

            selectors.Add("customer", c => c.Customer);
            selectors.Add("way-of-entry", c => c.WayOfEntry);
            selectors.Add("visit", x => x.Visit);
            selectors.Add("offer", x => x.Offer);

            // The selectors below won't work since we're not able to define the relationship in CrmContext
            // They are manually mapped in the DataProvider
            // selectors.Add("building", c => c.Building);
            // selectors.Add("contact", c => c.Contact);

            return source.Include<Request>(querySet, selectors);
        }

        public static IQueryable<Request> Sort(this IQueryable<Request> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Request, object>>>();

            selectors.Add("number", x => x.Id);
            selectors.Add("request-date", x => x.RequestDate);
            selectors.Add("employee", x => x.Employee);
            selectors.Add("customer.name", x => x.Customer.Name);
            selectors.Add("customer.street", x => x.Customer.Address1);
            selectors.Add("customer.postal-code", x => x.Customer.EmbeddedPostalCode);
            selectors.Add("customer.city", x => x.Customer.EmbeddedCity);
            selectors.Add("updated", x => x.Updated);

            return source.Sort<Request>(querySet, selectors);
        }

    }
}