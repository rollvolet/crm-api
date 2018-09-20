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
    public static class OrderQueryExtensions
    {
        public static IQueryable<Order> Filter(this IQueryable<Order> source, QuerySet querySet, CrmContext context)
        {
            if (querySet.Filter.Fields.ContainsKey("offer-number"))
            {
                var filterValue = querySet.Filter.Fields["offer-number"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.OfferNumber, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("offer.reference"))
            {
                var filterValue = querySet.Filter.Fields["offer.reference"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.Offer.Reference, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("request-number"))
            {
                var filterValue = querySet.Filter.Fields["request-number"];
                int number;
                if (Int32.TryParse(filterValue, out number)) {
                    var predicate = PredicateBuilder.New<Order>(x => x.RequestId == number);
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

            if (querySet.Filter.Fields.ContainsKey("invoice") && querySet.Filter.Fields["invoice"] == "false")
            {
                source = source.Where(e => e.Invoice == null);
            }

            return source;
        }

        public static IQueryable<Order> Include(this IQueryable<Order> source, QuerySet querySet)
        {
            if (querySet.Include.Fields.Contains("customer.honorific-prefix"))
                source = source.Include(x => x.Customer).ThenInclude(x => x.HonorificPrefix);

            if (querySet.Include.Fields.Contains("deposit-invoices"))
                source = source.Include(c => c.DepositInvoicesHubs).ThenInclude(d => d.DepositInvoice);

            var selectors = new Dictionary<string, Expression<Func<Order, object>>>();

            selectors.Add("customer", c => c.Customer);
            selectors.Add("offer", c => c.Offer);
            selectors.Add("invoice", c => c.Invoice);
            selectors.Add("vat-rate", c => c.VatRate);
            selectors.Add("deposits", c => c.Deposits);

            // dummy entries for resources that are already included
            selectors.Add("customer.honorific-prefix", null);
            selectors.Add("deposit-invoices", null);

            // The selectors below won't work since we're not able to define the relationship in CrmContext
            // They are manually mapped in the DataProvider
            // selectors.Add("building", c => c.Building);
            // selectors.Add("contact", c => c.Contact);
            selectors.Add("building", null);
            selectors.Add("contact", null);

            return source.Include<Order>(querySet, selectors);
        }

        public static IQueryable<Order> Sort(this IQueryable<Order> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Order, object>>>();

            selectors.Add("order-date", x => x.OrderDate);
            selectors.Add("offer-number", x => x.OfferNumber);
            selectors.Add("offer.reference", x => x.Offer.Reference);
            selectors.Add("customer.name", x => x.Customer.Name);
            selectors.Add("customer.street", x => x.Customer.Address1);
            selectors.Add("customer.postal-code", x => x.Customer.EmbeddedPostalCode);
            selectors.Add("customer.city", x => x.Customer.EmbeddedCity);

            return source.Sort<Order>(querySet, selectors);
        }

    }
}