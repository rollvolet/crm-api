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

            if (querySet.Filter.Fields.ContainsKey("origin.id"))
            {
                var filterValue = querySet.Filter.Fields["origin.id"];
                int orderId;
                if (Int32.TryParse(filterValue, out orderId))
                    source = source.Where(c => c.OriginId == orderId);
                else
                    throw new IllegalArgumentException("IllegalFilter", "Order id filter must be a integer.");
            }

            if (querySet.Filter.Fields.ContainsKey("hasInvoice"))
            {
                if (Int32.Parse(querySet.Filter.Fields["hasInvoice"]) == 0)
                    source = source.Where(e => e.Invoice.InterventionId == null);
                else if (Int32.Parse(querySet.Filter.Fields["hasInvoice"]) == 1)
                    source = source.Where(e => e.Invoice.InterventionId != null);
            }

            if (querySet.Filter.Fields.ContainsKey("isCancelled"))
            {
                if (Int32.Parse(querySet.Filter.Fields["isCancelled"]) == 0)
                    source = source.Where(e => e.CancellationDate == null && e.FollowUpRequest.OriginId == null);
                else if (Int32.Parse(querySet.Filter.Fields["isCancelled"]) == 1)
                    source = source.Where(e => e.CancellationDate != null || e.FollowUpRequest.OriginId != null);
            }

            if (querySet.Filter.Fields.ContainsKey("isPlanned"))
            {
                if (Int32.Parse(querySet.Filter.Fields["isPlanned"]) == 0)
                    source = source.Where(e => e.PlanningEvent.Date == null);
                else if (Int32.Parse(querySet.Filter.Fields["isPlanned"]) == 1)
                    source = source.Where(e => e.PlanningEvent.Date != null);
            }

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
            selectors.Add("building", c => c.Building);
            selectors.Add("contact", c => c.Contact);
            selectors.Add("way-of-entry", c => c.WayOfEntry);
            selectors.Add("employee", c => c.Employee);
            selectors.Add("invoice", x => x.Invoice);
            selectors.Add("origin", x => x.Origin);
            selectors.Add("follow-up-request", x => x.FollowUpRequest);
            selectors.Add("planning-event", x => x.PlanningEvent);

            // dummy entries for resources that are already included
            selectors.Add("customer.honorific-prefix", null);
            selectors.Add("customer.language", null);
            selectors.Add("technicians", null);

            return source.Include<Intervention>(querySet, selectors);
        }

        public static IQueryable<Intervention> Sort(this IQueryable<Intervention> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, IEnumerable<Expression<Func<Intervention, object>>>>();

            selectors.Add("number", new List<Expression<Func<Intervention, object>>> { x => x.Id });
            selectors.Add("date", new List<Expression<Func<Intervention, object>>> { x => x.Date, x => x.Id });
            selectors.Add("planning-event.date", new List<Expression<Func<Intervention, object>>> { x => x.PlanningEvent.Date, x => x.Date, x => x.Id });
            selectors.Add("customer.name", new List<Expression<Func<Intervention, object>>> { x => x.Customer.Name });
            selectors.Add("customer.street", new List<Expression<Func<Intervention, object>>> { x => x.Customer.Address1 });
            selectors.Add("customer.postal-code", new List<Expression<Func<Intervention, object>>> { x => x.Customer.EmbeddedPostalCode });
            selectors.Add("customer.city", new List<Expression<Func<Intervention, object>>> { x => x.Customer.EmbeddedCity });

            return source.Sort<Intervention>(querySet, selectors);
        }

        // Duplicated in each Case related query extension since EF core doesn't support generics
        public static IQueryable<Intervention> FilterCase(this IQueryable<Intervention> source, QuerySet querySet, CrmContext context)
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

            if (querySet.Filter.Fields.ContainsKey("customer.telephone"))
            {
                var search = querySet.Filter.Fields["customer.telephone"];
                var predicate = search.ConstructTelephoneQuery();

                source = source.AsExpandable().Where(c => c.Customer.Telephones.Any(t => predicate.Invoke(t)));
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