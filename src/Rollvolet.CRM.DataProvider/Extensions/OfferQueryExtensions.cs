using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Exceptions;
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

            if (querySet.Filter.Fields.ContainsKey("request-number"))
            {
                var filterValue = querySet.Filter.Fields["request-number"];
                int number;
                if (Int32.TryParse(filterValue, out number)) {
                    var predicate = PredicateBuilder.New<Offer>(x => x.RequestId == number);
                    var i = 10;
                    while (i * number < 1000000) {
                        var from = i * number;
                        var to = i * (number + 1);
                        predicate.Or(c => c.RequestId >= from && c.RequestId <= to);
                        i = i * 10;
                    }
                    source = source.Where(predicate);
                }
                else
                {
                    throw new IllegalArgumentException("IllegalFilter", "Request number filter must be a integer.");
                }
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

            if (querySet.Include.Fields.Contains("request.visit"))
                source = source.Include(x => x.Request).ThenInclude(x => x.Visit);

            if (querySet.Include.Fields.Contains("offerlines.vat-rate"))
                source = source.Include(x => x.Offerlines).ThenInclude(x => x.VatRate);

            var selectors = new Dictionary<string, Expression<Func<Offer, object>>>();

            selectors.Add("customer", c => c.Customer);
            selectors.Add("request", c => c.Request);
            selectors.Add("vat-rate", c => c.VatRate);
            selectors.Add("submission-type", c => c.SubmissionType);
            selectors.Add("offerlines", c => c.Offerlines);

            // dummy entries for resources that are already included
            selectors.Add("customer.honorific-prefix", null);
            selectors.Add("request.visit", null);
            selectors.Add("offerlines.vat-rate", null);

            // The selectors below won't work since we're not able to define the relationship in CrmContext
            // They are manually mapped in the DataProvider
            // selectors.Add("building", c => c.Building);
            // selectors.Add("contact", c => c.Contact);
            selectors.Add("building", null);
            selectors.Add("contact", null);

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