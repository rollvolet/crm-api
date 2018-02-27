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
    public static class InvoiceQueryExtensions
    {
        public static IQueryable<Invoice> Filter(this IQueryable<Invoice> source, QuerySet querySet, CrmContext context)  
        {
            if (querySet.Filter.Fields.ContainsKey("customer.name"))
            {
                var filterValue = querySet.Filter.Fields["customer.name"].FilterWhitespace().FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.CustomerSearchName, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("customer.postal-code"))
            {
                var filterValue = querySet.Filter.Fields["customer.postal-code"];
                source = source.Where(c => c.CustomerPostalCode == filterValue);
            }

            if (querySet.Filter.Fields.ContainsKey("customer.city"))
            {
                var filterValue = querySet.Filter.Fields["customer.city"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.CustomerCity, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("customer.street"))
            {
                var filterValue = querySet.Filter.Fields["customer.street"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.CustomerAddress1, filterValue)
                                        || EF.Functions.Like(c.CustomerAddress2, filterValue)
                                        || EF.Functions.Like(c.CustomerAddress3, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("customer.telephone"))
            {
                var search = querySet.Filter.Fields["customer.telephone"];

                if (search.StartsWith("+"))
                    search = search.Replace("+", "00");

                search = new Regex(@"[^\d]").Replace(search, ""); // only digits

                var fullNumber = search + "%";
                var paddedFullNumber = "0" + fullNumber;        

                source = source.Where(x => EF.Functions.Like((x.CustomerPhoneNumber).Replace(".", "").Replace("/", ""), fullNumber)
                                        || EF.Functions.Like(x.CustomerFaxNumber.Replace(".", "").Replace("/", ""), fullNumber)
                                        || EF.Functions.Like(x.CustomerMobileNumber.Replace(".", "").Replace("/", ""), fullNumber)
                                        || EF.Functions.Like((x.CustomerPhoneNumber).Replace(".", "").Replace("/", ""), paddedFullNumber)
                                        || EF.Functions.Like(x.CustomerFaxNumber.Replace(".", "").Replace("/", ""), paddedFullNumber)
                                        || EF.Functions.Like(x.CustomerMobileNumber.Replace(".", "").Replace("/", ""), paddedFullNumber));
            }

            if (querySet.Filter.Fields.ContainsKey("building.name"))
            {
                var filterValue = querySet.Filter.Fields["building.name"].FilterWhitespace().FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.BuildingSearchName, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("building.postal-code"))
            {
                var filterValue = querySet.Filter.Fields["building.postal-code"];
                source = source.Where(c => c.BuildingPostalCode == filterValue);
            }

            if (querySet.Filter.Fields.ContainsKey("building.city"))
            {
                var filterValue = querySet.Filter.Fields["building.city"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.BuildingCity, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("building.street"))
            {
                var filterValue = querySet.Filter.Fields["building.street"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.BuildingAddress1, filterValue)
                                        || EF.Functions.Like(c.BuildingAddress2, filterValue)
                                        || EF.Functions.Like(c.BuildingAddress3, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("number"))
            {
                var filterValue = querySet.Filter.Fields["number"];
                int number;
                if (Int32.TryParse(filterValue, out number)) {
                    var predicate = PredicateBuilder.New<Invoice>(x => x.Number == number);
                    var i = 10;
                    while (i * number < 1000000) {
                        var from = i * number;
                        var to = i * (number + 1);
                        predicate.Or(c => c.Number >= from && c.Number <= to);
                        i = i * 10;
                    }
                    source = source.Where(predicate);
                }
                // TODO throw exception about invalid number
            }

            if (querySet.Filter.Fields.ContainsKey("reference"))
            {
                var filterValue = querySet.Filter.Fields["reference"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.Reference, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("offer.number"))
            {
                var filterValue = querySet.Filter.Fields["offer.number"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.Order.OfferNumber, filterValue));
            }

            return source; 
        }      

        public static IQueryable<Invoice> Include(this IQueryable<Invoice> source, QuerySet querySet)
        {
            if (querySet.Include.Fields.Contains("customer.honorific-prefix"))
                source = source.Include(x => x.Customer).ThenInclude(x => x.HonorificPrefix);

            var selectors = new Dictionary<string, Expression<Func<Invoice, object>>>();

            selectors.Add("customer", c => c.Customer);
            selectors.Add("order", c => c.Order);
            selectors.Add("vat-rate", c => c.VatRate);
            selectors.Add("supplements", c => c.Supplements);

            // The selectors below won't work since we're not able to define the relationship in CrmContext
            // They are manually mapped in the DataProvider
            // selectors.Add("building", c => c.Building);
            // selectors.Add("contact", c => c.Contact);

            return source.Include<Invoice>(querySet, selectors);
        }

        public static IQueryable<Invoice> Sort(this IQueryable<Invoice> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Invoice, object>>>();

            selectors.Add("invoice-date", x => x.InvoiceDate);
            selectors.Add("year", x => x.Year);
            selectors.Add("number", x => x.Number);
            selectors.Add("reference", x => x.Reference);
            selectors.Add("offer.number", x => x.Order.OfferNumber);
            selectors.Add("customer.name", x => x.CustomerName);
            selectors.Add("customer.street", x => x.CustomerAddress1);
            selectors.Add("customer.postal-code", x => x.CustomerPostalCode);
            selectors.Add("customer.city", x => x.CustomerCity);
            selectors.Add("updated", x => x.Updated);

            return source.Sort<Invoice>(querySet, selectors);
        }

    }
}