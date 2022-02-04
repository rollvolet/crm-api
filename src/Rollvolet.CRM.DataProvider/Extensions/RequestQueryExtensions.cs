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
    public static class RequestQueryExtensions
    {
        public static IQueryable<Request> Filter(this IQueryable<Request> source, QuerySet querySet, CrmContext context)
        {
            if (querySet.Filter.Fields.ContainsKey("number"))
            {
                var filterValue = querySet.Filter.Fields["number"];
                if (!String.IsNullOrEmpty(filterValue))
                {
                    int number;
                    if (Int32.TryParse(filterValue, out number)) {
                        var predicate = PredicateBuilder.New<Request>(c => c.Id == number);
                        var i = 10;
                        while (i * number < 1000000) {
                            var from = i * number;
                            var to = i * (number + 1);
                            predicate.Or(c => c.Id >= from && c.Id < to);
                            i = i * 10;
                        }
                        source = source.Where(predicate);
                    }
                    else
                    {
                        throw new IllegalArgumentException("IllegalFilter", "Number filter must be a integer.");
                    }
                }
            }

            if (querySet.Filter.Fields.ContainsKey("visitor"))
            {
                var filterValue = querySet.Filter.Fields["visitor"].FilterWildcard();
                source = source.Where(e => EF.Functions.Like(e.Visit.Visitor, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("offer") && querySet.Filter.Fields["offer"] == "false")
                source = source.Where(e => e.Offer == null);

            source = source.FilterCase(querySet, context);

            return source;
        }

        public static IQueryable<Request> Include(this IQueryable<Request> source, QuerySet querySet)
        {
            if (querySet.Include.Fields.Contains("customer"))
                source = source.Include(x => x.Customer).ThenInclude(x => x.Memo);
            if (querySet.Include.Fields.Contains("customer.honorific-prefix"))
                source = source.Include(x => x.Customer).ThenInclude(x => x.HonorificPrefix);
            if (querySet.Include.Fields.Contains("customer.language"))
                source = source.Include(x => x.Customer).ThenInclude(x => x.Language);

            var selectors = new Dictionary<string, Expression<Func<Request, object>>>();

            selectors.Add("building", c => c.Building);
            selectors.Add("contact", c => c.Contact);
            selectors.Add("way-of-entry", c => c.WayOfEntry);
            selectors.Add("calendar-event", x => x.Visit);
            selectors.Add("offer", x => x.Offer);
            selectors.Add("origin", x => x.Origin);

            // dummy entries for resources that are already included
            selectors.Add("customer", null);
            selectors.Add("customer.honorific-prefix", null);
            selectors.Add("customer.language", null);

            return source.Include<Request>(querySet, selectors);
        }

        public static IQueryable<Request> Sort(this IQueryable<Request> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, IEnumerable<Expression<Func<Request, object>>>>();

            selectors.Add("number", new List<Expression<Func<Request, object>>> { x => x.Id });
            selectors.Add("request-date", new List<Expression<Func<Request, object>>> { x => x.RequestDate, x => x.Id });
            selectors.Add("employee", new List<Expression<Func<Request, object>>> { x => x.Employee });
            selectors.Add("customer.name", new List<Expression<Func<Request, object>>> { x => x.Customer.Name });
            selectors.Add("customer.street", new List<Expression<Func<Request, object>>> { x => x.Customer.Address1 });
            selectors.Add("customer.postal-code", new List<Expression<Func<Request, object>>> { x => x.Customer.EmbeddedPostalCode });
            selectors.Add("customer.city", new List<Expression<Func<Request, object>>> { x => x.Customer.EmbeddedCity });

            return source.Sort<Request>(querySet, selectors);
        }

        // Duplicated in each Case related query extension since EF core doesn't support generics
        public static IQueryable<Request> FilterCase(this IQueryable<Request> source, QuerySet querySet, CrmContext context)
        {
            if (querySet.Filter.Fields.ContainsKey("customer.number"))
            {
                var filterValue = querySet.Filter.Fields["customer.number"];
                if (!String.IsNullOrEmpty(filterValue))
                {
                    int number;
                    if (Int32.TryParse(filterValue, out number)) {
                        source = source.Where(c => c.Customer.Number == number);
                    } else
                    {
                        throw new IllegalArgumentException("IllegalFilter", "Customer number filter must be a integer.");
                    }
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

            var buildingFilters = querySet.Filter.Fields.Keys.Where(k => k.StartsWith("building"));

            if (buildingFilters.Count() > 0)
            {
                if (querySet.Filter.Fields.ContainsKey("building.name"))
                {
                    var filterValue = querySet.Filter.Fields["building.name"].TextSearch();
                    source = source.Where(c => EF.Functions.Like(c.Building.SearchName, filterValue));
                }

                if (querySet.Filter.Fields.ContainsKey("building.postal-code"))
                {
                    var filterValue = querySet.Filter.Fields["building.postal-code"].FilterWildcard();
                    source = source.Where(c => EF.Functions.Like(c.Building.EmbeddedPostalCode, filterValue));
                }

                if (querySet.Filter.Fields.ContainsKey("building.city"))
                {
                    var filterValue = querySet.Filter.Fields["building.city"].FilterWildcard();
                    source = source.Where(c => EF.Functions.Like(c.Building.EmbeddedCity, filterValue));
                }

                if (querySet.Filter.Fields.ContainsKey("building.street"))
                {
                    var filterValue = querySet.Filter.Fields["building.street"].FilterWildcard();
                    source = source.Where(c => EF.Functions.Like(c.Building.Address1, filterValue)
                                            || EF.Functions.Like(c.Building.Address2, filterValue)
                                            || EF.Functions.Like(c.Building.Address3, filterValue));
                }
            }

            return source;
        }
    }
}