using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProvider.Extensions
{
    public static class RequestQueryExtensions
    {
        public static IQueryable<Request> Filter(this IQueryable<Request> source, QuerySet querySet)  
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
                // TODO merge with telephone query in CustomerRecordExtensions

                var search = querySet.Filter.Fields["customer.telephone"];

                if (search.StartsWith("+"))
                    search = search.Replace("+", "00");

                search = new Regex(@"[^\d]").Replace(search, ""); // only digits

                // Check if search starts with a valid country code
                // If so, match the remaining part and 0... with Zone+Tel and Tel
                // Else, match the full search and 0... with Zone+Tel and Tel
                var countryCode = search.Length >= 4 ? search.Substring(0, 4) : "9999"; // TODO: country code may contain more or less than 4 chars
                var fullNumber = search + "%";
                var paddedFullNumber = "0" + fullNumber;
                var numberWithoutCountry = search.Length >= 4 ? fullNumber.Substring(4) : fullNumber;
                var paddedNumberWithoutCountry = "0" + numberWithoutCountry;

                source = source.Where(c => c.Customer.Telephones.Any(t =>
                    (
                        t.Country.TelephonePrefix == countryCode && (
                            EF.Functions.Like((t.Area + t.Number).Replace(".", ""), numberWithoutCountry)
                            || EF.Functions.Like(t.Number.Replace(".", ""), numberWithoutCountry)
                            || EF.Functions.Like((t.Area + t.Number).Replace(".", ""), paddedNumberWithoutCountry)
                            || EF.Functions.Like(t.Number.Replace(".", ""), paddedNumberWithoutCountry)
                        )
                    ) || (
                        EF.Functions.Like((t.Area + t.Number).Replace(".", ""), fullNumber)
                        || EF.Functions.Like(t.Number.Replace(".", ""), fullNumber)
                        || EF.Functions.Like((t.Area + t.Number).Replace(".", ""), paddedFullNumber)
                        || EF.Functions.Like(t.Number.Replace(".", ""), paddedFullNumber)
                    )
                ));
            }            

            if (querySet.Filter.Fields.ContainsKey("building.name"))
            {
                var filterValue = querySet.Filter.Fields["building.name"].FilterWhitespace().FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.Building.SearchName, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("building.postal-code"))
            {
                var filterValue = querySet.Filter.Fields["building.postal-code"];
                source = source.Where(c => c.Building.EmbeddedPostalCode == filterValue);
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

            // TODO filter met / zonder offerte

            return source;
        }      

        public static IQueryable<Request> Include(this IQueryable<Request> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Request, object>>>();

            selectors.Add("customer", c => c.Customer);
            selectors.Add("way-of-entry", c => c.WayOfEntry);

            // The selectors below won't work since we're not able to define the relationship in CrmContext
            // selectors.Add("building", c => c.Building);
            // selectors.Add("contact", c => c.Contact);

            return source.Include<Request>(querySet, selectors);
        }

        public static IQueryable<Request> Sort(this IQueryable<Request> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Request, object>>>();

            selectors.Add("request-date", x => x.RequestDate);
            selectors.Add("employee", x => x.Employee);

            return source.Sort<Request>(querySet, selectors);
        }

    }
}