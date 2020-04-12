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

            if (querySet.Filter.Fields.ContainsKey("request.visitor"))
            {
                var filterValue = querySet.Filter.Fields["request.visitor"].FilterWildcard();
                source = source.Where(e => EF.Functions.Like(e.Request.Visit.Visitor, filterValue));
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

            if (querySet.Include.Fields.Contains("request") || querySet.Include.Fields.Contains("request.calendar-event"))
                source = source.Include(x => x.Request).ThenInclude(x => x.Visit);

            if (querySet.Include.Fields.Contains("offerlines.vat-rate"))
                source = source.Include(x => x.Offerlines).ThenInclude(x => x.VatRate);

            var selectors = new Dictionary<string, Expression<Func<Offer, object>>>();

            selectors.Add("customer", c => c.Customer);
            selectors.Add("vat-rate", c => c.VatRate);
            selectors.Add("offerlines", c => c.Offerlines);

            // dummy entries for resources that are already included
            selectors.Add("customer.honorific-prefix", null);
            selectors.Add("request", null);
            selectors.Add("request.calendar-event", null);
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

        // Duplicated in each Case related query extension since EF core doesn't support generics
        public static IQueryable<Offer> FilterCase(this IQueryable<Offer> source, QuerySet querySet, CrmContext context)
        {
            if (querySet.Filter.Fields.ContainsKey("customer.number"))
            {
                var filterValue = querySet.Filter.Fields["customer.number"];
                int number;
                if (Int32.TryParse(filterValue, out number)) {
                    source = source.Where(c => c.Customer.Number == number);
                } else
                {
                    throw new IllegalArgumentException("IllegalFilter", "Customer number filter must be a integer.");
                }
            }

            if (querySet.Filter.Fields.ContainsKey("customer.name"))
            {
                var filterValue = querySet.Filter.Fields["customer.name"].TextSearch();
                source = source.Where(c => EF.Functions.Like(c.Customer.SearchName, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("customer.postal-code"))
            {
                var filterValue = querySet.Filter.Fields["customer.postal-code"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.Customer.EmbeddedPostalCode, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("customer.city"))
            {
                var filterValue = querySet.Filter.Fields["customer.city"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.Customer.EmbeddedCity, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("customer.street"))
            {
                var filterValue = querySet.Filter.Fields["customer.street"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.Customer.Address1, filterValue)
                                        || EF.Functions.Like(c.Customer.Address2, filterValue)
                                        || EF.Functions.Like(c.Customer.Address3, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("customer.telephone"))
            {
                var search = querySet.Filter.Fields["customer.telephone"];
                var predicate = search.ConstructTelephoneQuery();

                source = source.AsExpandable().Where(c => c.Customer.Telephones.Any(t => predicate.Invoke(t)));
            }

            var buildingFilters = querySet.Filter.Fields.Keys.Where(k => k.StartsWith("building"));

            if (buildingFilters.Count() > 0)
            {
                var predicate = PredicateBuilder.New<CaseTuple<Offer>>(true);

                if (querySet.Filter.Fields.ContainsKey("building.name"))
                {
                    var filterValue = querySet.Filter.Fields["building.name"].TextSearch();
                    predicate.And(c => EF.Functions.Like(c.Building.SearchName, filterValue));
                }

                if (querySet.Filter.Fields.ContainsKey("building.postal-code"))
                {
                    var filterValue = querySet.Filter.Fields["building.postal-code"].FilterWildcard();
                    predicate.And(c => EF.Functions.Like(c.Building.EmbeddedPostalCode, filterValue));
                }

                if (querySet.Filter.Fields.ContainsKey("building.city"))
                {
                    var filterValue = querySet.Filter.Fields["building.city"].FilterWildcard();
                    predicate.And(c => EF.Functions.Like(c.Building.EmbeddedCity, filterValue));
                }

                if (querySet.Filter.Fields.ContainsKey("building.street"))
                {
                    var filterValue = querySet.Filter.Fields["building.street"].FilterWildcard();
                    predicate.And(c => EF.Functions.Like(c.Building.Address1, filterValue)
                                            || EF.Functions.Like(c.Building.Address2, filterValue)
                                            || EF.Functions.Like(c.Building.Address3, filterValue));
                }

                source = source.Join(
                    context.Buildings,
                    r => new { Number = r.RelativeBuildingId, CustomerId = r.CustomerId },
                    b => new { Number = (int?) b.Number, CustomerId = (int?) b.CustomerId },
                    (r, b) => new CaseTuple<Offer> { Source = r, Building = b }
                ).Where(predicate)
                .Select(x => x.Source);
            }

            return source;
        }
    }
}