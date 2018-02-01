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
                source = source.Where(c => EF.Functions.Like(c.Offer.Reference, filterValue));
            }

            source = source.FilterCase(querySet, context);

            // TODO filter met / zonder factuur

            return source;
        }      

        public static IQueryable<Order> Include(this IQueryable<Order> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Order, object>>>();

            selectors.Add("customer", c => c.Customer);
            selectors.Add("offer", c => c.Offer);
            selectors.Add("vat-rate", c => c.VatRate);
            selectors.Add("deposits", c => c.Deposits);

            // The selectors below won't work since we're not able to define the relationship in CrmContext
            // They are manually mapped in the DataProvider
            // selectors.Add("building", c => c.Building);
            // selectors.Add("contact", c => c.Contact);

            return source.Include<Order>(querySet, selectors);
        }

        public static IQueryable<Order> Sort(this IQueryable<Order> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Order, object>>>();

            selectors.Add("order-date", x => x.OrderDate);
            selectors.Add("offer-number", x => x.OfferNumber);
            selectors.Add("customer.name", x => x.Customer.Name);
            selectors.Add("customer.street", x => x.Customer.Address1);
            selectors.Add("customer.postal-code", x => x.Customer.EmbeddedPostalCode);
            selectors.Add("customer.city", x => x.Customer.EmbeddedCity);
            selectors.Add("updated", x => x.Updated);

            return source.Sort<Order>(querySet, selectors);
        }

    }
}