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

            if (querySet.Filter.Fields.ContainsKey("customer.name"))
            {
                var filterValue = querySet.Filter.Fields["customer.name"].FilterWhitespace().FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.Customer.SearchName, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("customer.postal-code"))
            {
                var filterValue = querySet.Filter.Fields["customer.postal-code"];
                source = source.Where(c => c.Customer.EmbeddedPostalCode == filterValue);
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
                var predicate = PredicateBuilder.New<Request.BuildingTuple>(true);

                if (querySet.Filter.Fields.ContainsKey("building.name"))
                {
                    var filterValue = querySet.Filter.Fields["building.name"].FilterWhitespace().FilterWildcard();
                    predicate.And(c => EF.Functions.Like(c.Building.SearchName, filterValue));
                }

                if (querySet.Filter.Fields.ContainsKey("building.postal-code"))
                {
                    var filterValue = querySet.Filter.Fields["building.postal-code"];
                    predicate.And(c => c.Building.EmbeddedPostalCode == filterValue);
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
                    (r, b) => new Request.BuildingTuple { Request = r, Building = b }
                ).Where(predicate)
                .Select(x => x.Request);
            }

            // TODO filter met / zonder offerte

            return source;
        }      

        public static IQueryable<Request> Include(this IQueryable<Request> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Request, object>>>();

            selectors.Add("customer", c => c.Customer);
            selectors.Add("way-of-entry", c => c.WayOfEntry);

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