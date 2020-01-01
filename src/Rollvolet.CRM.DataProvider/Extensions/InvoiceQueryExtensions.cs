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
    public static class InvoiceQueryExtensions
    {
        public static IQueryable<Invoice> Filter(this IQueryable<Invoice> source, QuerySet querySet, CrmContext context, bool isDepositInvoice = false)
        {
            if (querySet.Filter.Fields.ContainsKey("number"))
            {
                var filterValue = querySet.Filter.Fields["number"].Replace("/", "");
                int number;
                if (Int32.TryParse(filterValue, out number))
                 {
                    var predicate = PredicateBuilder.New<Invoice>(x => x.Number == number);
                    var i = 10;
                    while (i * number < 10000000) {
                        var from = i * number;
                        var to = i * (number + 1);
                        predicate.Or(c => c.Number >= from && c.Number <= to);
                        i = i * 10;
                    }
                    source = source.Where(predicate);
                }
                else
                {
                    throw new IllegalArgumentException("IllegalFilter", "Number filter must be a integer.");
                }
            }

            if (querySet.Filter.Fields.ContainsKey("reference"))
            {
                var filterValue = querySet.Filter.Fields["reference"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.Reference, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("order.id"))
            {
                var filterValue = querySet.Filter.Fields["order.id"];
                int orderId;
                if (Int32.TryParse(filterValue, out orderId))
                {
                    if (isDepositInvoice)
                        source = source.Where(c => c.MainInvoiceHub.OrderId == orderId);
                    else
                        source = source.Where(c => c.OrderId == orderId);
                }
                else
                {
                    throw new IllegalArgumentException("IllegalFilter", "Order id filter must be a integer.");
                }
            }

            if (querySet.Filter.Fields.ContainsKey("offer.number"))
            {
                var filterValue = querySet.Filter.Fields["offer.number"].FilterWildcard();

                if (isDepositInvoice)
                    source = source.Where(c => EF.Functions.Like(c.MainInvoiceHub.Order.OfferNumber, filterValue));
                else
                    source = source.Where(c => EF.Functions.Like(c.Order.OfferNumber, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("offer.request-number"))
            {
                var filterValue = querySet.Filter.Fields["offer.request-number"];
                int number;
                if (Int32.TryParse(filterValue, out number)) {
                    if (isDepositInvoice)
                    {
                        var predicate = PredicateBuilder.New<Invoice>(x => x.MainInvoiceHub.Order.RequestId == number);
                        var i = 10;
                        while (i * number < 1000000) {
                            var from = i * number;
                            var to = i * (number + 1);
                            predicate.Or(c => c.MainInvoiceHub.Order.RequestId >= from && c.MainInvoiceHub.Order.RequestId <= to);
                            i = i * 10;
                        }
                        source = source.Where(predicate);
                    }
                    else
                    {
                        var predicate = PredicateBuilder.New<Invoice>(x => x.Order.RequestId == number);
                        var i = 10;
                        while (i * number < 1000000) {
                            var from = i * number;
                            var to = i * (number + 1);
                            predicate.Or(c => c.Order.RequestId >= from && c.Order.RequestId <= to);
                            i = i * 10;
                        }
                        source = source.Where(predicate);
                    }
                }
                else
                {
                    throw new IllegalArgumentException("IllegalFilter", "Request number filter must be a integer.");
                }
            }

            source = source.FilterCase(querySet, context);

            return source;
        }

        public static IQueryable<Invoice> Include(this IQueryable<Invoice> source, QuerySet querySet, bool isDepositInvoice = false)
        {
            if (querySet.Include.Fields.Contains("customer.honorific-prefix"))
                source = source.Include(x => x.Customer).ThenInclude(x => x.HonorificPrefix);

            if (querySet.Include.Fields.Contains("supplements.unit"))
                source = source.Include(x => x.Supplements).ThenInclude(x => x.Unit);

            var selectors = new Dictionary<string, Expression<Func<Invoice, object>>>();

            selectors.Add("customer", c => c.Customer);
            selectors.Add("vat-rate", c => c.VatRate);

            // dummy entries for resources that are already included
            selectors.Add("customer.honorific-prefix", null);
            selectors.Add("supplements.unit", null);

            if (!isDepositInvoice) // only available on normal invoices
            {
                if (querySet.Include.Fields.Contains("deposit-invoices"))
                    source = source.Include(c => c.DepositInvoiceHubs).ThenInclude(d => d.DepositInvoice);
                if (querySet.Include.Fields.Contains("working-hours.employee"))
                    source = source.Include(c => c.WorkingHours).ThenInclude(h => h.Employee);

                selectors.Add("order", c => c.Order);
                selectors.Add("supplements", c => c.Supplements);
                selectors.Add("deposits", c => c.Deposits);
                selectors.Add("working-hours", c => c.WorkingHours);

                selectors.Add("deposit-invoices", null);
                selectors.Add("working-hours.employee", null);
            }
            else
            {
                if (querySet.Include.Fields.Contains("order"))
                    source = source.Include(c => c.MainInvoiceHub).ThenInclude(d => d.Order);
            }

            // The selectors below won't work since we're not able to define the relationship in CrmContext
            // They are manually mapped in the DataProvider
            // selectors.Add("building", c => c.Building);
            // selectors.Add("contact", c => c.Contact);
            selectors.Add("building", null);
            selectors.Add("contact", null);

            return source.Include<Invoice>(querySet, selectors);
        }

        public static IQueryable<Invoice> Sort(this IQueryable<Invoice> source, QuerySet querySet, bool isDepositInvoice = false)
        {
            var selectors = new Dictionary<string, Expression<Func<Invoice, object>>>();

            selectors.Add("invoice-date", x => x.InvoiceDate);
            selectors.Add("year", x => x.Year);
            selectors.Add("number", x => x.Number);
            selectors.Add("reference", x => x.Reference);
            selectors.Add("offer.number", x => isDepositInvoice ? x.MainInvoiceHub.Order.OfferNumber : x.Order.OfferNumber);
            selectors.Add("customer.name", x => x.CustomerName);
            selectors.Add("customer.street", x => x.CustomerAddress1);
            selectors.Add("customer.postal-code", x => x.CustomerPostalCode);
            selectors.Add("customer.city", x => x.CustomerCity);

            return source.Sort<Invoice>(querySet, selectors);
        }

    }
}