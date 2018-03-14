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
    public static class OfferQueryExtensions
    {
        public static IQueryable<Offer> Filter(this IQueryable<Offer> source, QuerySet querySet, CrmContext context)  
        {
            if (querySet.Filter.Fields.ContainsKey("number"))
            {
                var filterValue = querySet.Filter.Fields["number"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.Number, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("reference"))
            {
                var filterValue = querySet.Filter.Fields["reference"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.Reference, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("request.number"))
            {
                var filterValue = querySet.Filter.Fields["request.number"];
                int number;
                if (Int32.TryParse(filterValue, out number)) {
                    var predicate = PredicateBuilder.New<Offer>(x => x.Request.Id == number);
                    var i = 10;
                    while (i * number < 1000000) {
                        var from = i * number;
                        var to = i * (number + 1);
                        predicate.Or(c => c.Request.Id >= from && c.Request.Id <= to);
                        i = i * 10;
                    }
                    source = source.Where(predicate);
                }
                // TODO throw exception about invalid number
            }

            source = source.FilterCase(querySet, context);

            if (querySet.Filter.Fields.ContainsKey("order") && querySet.Filter.Fields["order"] == "false")
            {
                source = source.Where(e => e.Order == null);
            }

            return source;
        }      

        public static IQueryable<Offer> Include(this IQueryable<Offer> source, QuerySet querySet)
        {
            if (querySet.Include.Fields.Contains("customer.honorific-prefix"))
                source = source.Include(x => x.Customer).ThenInclude(x => x.HonorificPrefix);

            var selectors = new Dictionary<string, Expression<Func<Offer, object>>>();

            selectors.Add("customer", c => c.Customer);
            selectors.Add("request", c => c.Request);
            selectors.Add("vat-rate", c => c.VatRate);
            selectors.Add("submission-type", c => c.SubmissionType);

            // The selectors below won't work since we're not able to define the relationship in CrmContext
            // They are manually mapped in the DataProvider
            // selectors.Add("building", c => c.Building);
            // selectors.Add("contact", c => c.Contact);

            return source.Include<Offer>(querySet, selectors);
        }

        public static IQueryable<Offer> Sort(this IQueryable<Offer> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Offer, object>>>();

            selectors.Add("number", x => x.Number);
            selectors.Add("reference", x => x.Reference);
            selectors.Add("offer-date", x => x.OfferDate);
            selectors.Add("customer.name", x => x.Customer.Name);
            selectors.Add("customer.street", x => x.Customer.Address1);
            selectors.Add("customer.postal-code", x => x.Customer.EmbeddedPostalCode);
            selectors.Add("customer.city", x => x.Customer.EmbeddedCity);

            return source.Sort<Offer>(querySet, selectors);
        }

    }
}