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
                if (!String.IsNullOrEmpty(filterValue))
                {
                    int number;
                    if (Int32.TryParse(filterValue, out number))
                    {
                        var predicate = PredicateBuilder.New<Invoice>(x => x.Number == number);
                        var i = 10;
                        while (i * number < 10000000) {
                            var from = i * number;
                            var to = i * (number + 1);
                            predicate.Or(c => c.Number >= from && c.Number < to);
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

            if (querySet.Filter.Fields.ContainsKey("reference"))
            {
                var filterValue = querySet.Filter.Fields["reference"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.Reference, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("order.id"))
            {
                var filterValue = querySet.Filter.Fields["order.id"];
                if (!String.IsNullOrEmpty(filterValue))
                {
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
                if (!String.IsNullOrEmpty(filterValue))
                {
                    int number;
                    if (Int32.TryParse(filterValue, out number)) {
                        if (isDepositInvoice)
                        {
                            var predicate = PredicateBuilder.New<Invoice>(x => x.MainInvoiceHub.Order.RequestId == number);
                            var i = 10;
                            while (i * number < 1000000) {
                                var from = i * number;
                                var to = i * (number + 1);
                                predicate.Or(c => c.MainInvoiceHub.Order.RequestId >= from && c.MainInvoiceHub.Order.RequestId < to);
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
                                predicate.Or(c => c.Order.RequestId >= from && c.Order.RequestId < to);
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
            }

            if (querySet.Filter.Fields.ContainsKey("certificate-received") && querySet.Filter.Fields["certificate-received"] == "true")
            {
                source = source.Where(e => e.CertificateReceived);
            }

            source = source.FilterCase(querySet, context);

            return source;
        }

        public static IQueryable<Invoice> Include(this IQueryable<Invoice> source, QuerySet querySet, bool isDepositInvoice = false)
        {
            if (querySet.Include.Fields.Contains("customer"))
                source = source.Include(x => x.Customer).ThenInclude(x => x.Memo);

            if (querySet.Include.Fields.Contains("customer.honorific-prefix"))
                source = source.Include(x => x.Customer).ThenInclude(x => x.HonorificPrefix);

            var selectors = new Dictionary<string, Expression<Func<Invoice, object>>>();

            selectors.Add("building", c => c.Building);
            selectors.Add("contact", c => c.Contact);
            selectors.Add("vat-rate", c => c.VatRate);

            // dummy entries for resources that are already included
            selectors.Add("customer", null);
            selectors.Add("customer.honorific-prefix", null);

            if (!isDepositInvoice) // only available on normal invoices
            {
                if (querySet.Include.Fields.Contains("deposit-invoices"))
                    source = source.Include(c => c.DepositInvoiceHubs).ThenInclude(d => d.DepositInvoice);
                if (querySet.Include.Fields.Contains("working-hours.employee"))
                    source = source.Include(c => c.WorkingHours).ThenInclude(h => h.Employee);

                selectors.Add("order", c => c.Order);
                selectors.Add("intervention", c => c.Intervention);
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

        // Duplicated in each Case related query extension since EF core doesn't support generics
        public static IQueryable<Invoice> FilterCase(this IQueryable<Invoice> source, QuerySet querySet, CrmContext context)
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