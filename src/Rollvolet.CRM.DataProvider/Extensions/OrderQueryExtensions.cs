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
    public static class OrderQueryExtensions
    {
        public static IQueryable<Order> Filter(this IQueryable<Order> source, QuerySet querySet, CrmContext context)
        {
            if (querySet.Filter.Fields.ContainsKey("offer-number"))
            {
                var filterValue = querySet.Filter.Fields["offer-number"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.OfferNumber, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("reference"))
            {
                var filterValue = querySet.Filter.Fields["reference"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.Reference, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("request-number"))
            {
                var filterValue = querySet.Filter.Fields["request-number"];
                if (!String.IsNullOrEmpty(filterValue))
                {
                    int number;
                    if (Int32.TryParse(filterValue, out number)) {
                        var predicate = PredicateBuilder.New<Order>(x => x.RequestId == number);
                        var i = 10;
                        while (i * number < 1000000) {
                            var from = i * number;
                            var to = i * (number + 1);
                            predicate.Or(c => c.RequestId >= from && c.RequestId < to);
                            i = i * 10;
                        }
                        source = source.Where(predicate);
                    }
                    else
                    {
                        throw new IllegalArgumentException("IllegalFilter", "Request number filter must be a integer.");
                    }
                }
            }

            if (querySet.Filter.Fields.ContainsKey("offer.request.visitor"))
            {
                var filterValue = querySet.Filter.Fields["offer.request.visitor"].FilterWildcard();
                source = source.Where(e => EF.Functions.Like(e.Offer.Request.Visit.Visitor, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("hasInvoice"))
            {
                if (Int32.Parse(querySet.Filter.Fields["hasInvoice"]) == 0)
                    source = source.Where(e => e.Invoice == null);
                else if (Int32.Parse(querySet.Filter.Fields["hasInvoice"]) == 1)
                    source = source.Where(e => e.Invoice != null);
            }

            if (querySet.Filter.Fields.ContainsKey("isCancelled"))
            {
                if (Int32.Parse(querySet.Filter.Fields["isCancelled"]) == 0)
                    source = source.Where(e => !e.Canceled);
                else if (Int32.Parse(querySet.Filter.Fields["isCancelled"]) == 1)
                    source = source.Where(e => e.Canceled);
            }

            source = source.FilterCase(querySet, context);

            return source;
        }

        public static IQueryable<Order> Include(this IQueryable<Order> source, QuerySet querySet)
        {
            if (querySet.Include.Fields.Contains("customer"))
                source = source.Include(x => x.Customer).ThenInclude(x => x.Memo);

            if (querySet.Include.Fields.Contains("customer.honorific-prefix"))
                source = source.Include(x => x.Customer).ThenInclude(x => x.HonorificPrefix);

            if (querySet.Include.Fields.Contains("deposit-invoices"))
                source = source.Include(c => c.DepositInvoicesHubs).ThenInclude(d => d.DepositInvoice);

            if (querySet.Include.Fields.Contains("technicians"))
                source = source.Include(x => x.OrderTechnicians).ThenInclude(x => x.Employee);

            var selectors = new Dictionary<string, Expression<Func<Order, object>>>();

            selectors.Add("building", c => c.Building);
            selectors.Add("contact", c => c.Contact);
            selectors.Add("offer", c => c.Offer);
            selectors.Add("invoice", c => c.Invoice);
            selectors.Add("vat-rate", c => c.VatRate);
            selectors.Add("deposits", c => c.Deposits);
            selectors.Add("interventions", c => c.Interventions);

            // dummy entries for resources that are already included
            selectors.Add("customer", null);
            selectors.Add("customer.honorific-prefix", null);
            selectors.Add("deposit-invoices", null);
            selectors.Add("technicians", null);

            return source.Include<Order>(querySet, selectors);
        }

        public static IQueryable<Order> Sort(this IQueryable<Order> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Order, object>>>();

            selectors.Add("order-date", x => x.OrderDate);
            selectors.Add("offer-number", x => x.OfferNumber);
            selectors.Add("reference", x => x.Reference);
            selectors.Add("customer.name", x => x.Customer.Name);
            selectors.Add("customer.street", x => x.Customer.Address1);
            selectors.Add("customer.postal-code", x => x.Customer.EmbeddedPostalCode);
            selectors.Add("customer.city", x => x.Customer.EmbeddedCity);

            return source.Sort<Order>(querySet, selectors);
        }

        // Duplicated in each Case related query extension since EF core doesn't support generics
        public static IQueryable<Order> FilterCase(this IQueryable<Order> source, QuerySet querySet, CrmContext context)
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

            if (querySet.Filter.Fields.ContainsKey("customer.telephone")) // telephones contain comma-seperated list of customer ids
            {
                if (String.IsNullOrEmpty(querySet.Filter.Fields["customer.telephone"]))
                {
                    // No telephone-numbers are found, so force that no matching result will be returned
                    source = source.Where(c =>  c.Id < 0);
                }
                else
                {
                    var ids = querySet.Filter.Fields["customer.telephone"].Split(",").Select(int.Parse).ToList();
                    source = source.Where(c => ids.Contains(c.Customer.DataId));
                }
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