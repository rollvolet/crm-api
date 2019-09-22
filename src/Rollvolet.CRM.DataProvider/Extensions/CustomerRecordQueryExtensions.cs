using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProvider.Extensions
{
    public static class CustomerRecordQueryExtensions
    {
        public static IQueryable<Customer> Filter(this IQueryable<Customer> source, QuerySet querySet)
        {
            source = source.Filter<Customer>(querySet);

            if (querySet.Filter.Fields.ContainsKey("number"))
            {
                var filterValue = querySet.Filter.Fields["number"];
                int number;
                if (Int32.TryParse(filterValue, out number)) {
                    var predicate = PredicateBuilder.New<Customer>(c => c.Number == number);
                    var i = 10;
                    while (i * number < 1000000) {
                        var from = i * number;
                        var to = i * (number + 1);
                        predicate.Or(c => c.Number >= from && c.Number <= to);
                        i = i * 10;
                    }
                    source = source.Where(predicate);
                } else
                {
                    throw new IllegalArgumentException("IllegalFilter", "Number filter must be a integer.");
                }
            }

            return source;
        }

        public static IQueryable<Customer> Include(this IQueryable<Customer> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Customer, object>>>();

            selectors.Add("country", c => c.Country);
            selectors.Add("language", c => c.Language);
            selectors.Add("honorific-prefix", c => c.HonorificPrefix);
            selectors.Add("contacts", c => c.Contacts);
            selectors.Add("buildings", c => c.Buildings);
            selectors.Add("telephones", c => c.Telephones);
            selectors.Add("requests", c => c.Requests);
            selectors.Add("offers", c => c.Offers);
            selectors.Add("invoices", c => c.Invoices);

            source = source.Include<Customer>(querySet, selectors);

            if (querySet.Include.Fields.Contains("tags"))
            {
                source = source.Include(c => c.CustomerTags).ThenInclude(ct => ct.Tag);
            }

            return source;
        }

        public static IQueryable<Customer> Sort(this IQueryable<Customer> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Customer, object>>>();

            selectors.Add("name", x => x.SearchName);
            selectors.Add("number", x => x.Number);
            selectors.Add("address", x => x.Address1);
            selectors.Add("postal-code", x => x.EmbeddedPostalCode);
            selectors.Add("city", x => x.EmbeddedCity);
            selectors.Add("created", x => x.Created);

            return source.Sort<Customer>(querySet, selectors);
        }

        public static IQueryable<Contact> Filter(this IQueryable<Contact> source, QuerySet querySet)
        {
            source = source.Filter<Contact>(querySet);

            if (querySet.Filter.Fields.ContainsKey("number"))
            {
                var filterValue = querySet.Filter.Fields["number"];
                int number;
                if (Int32.TryParse(filterValue, out number))
                    source = source.Where(c => c.Number == number);
            }

            if (querySet.Filter.Fields.ContainsKey("customer"))
            {
                var filterValue = querySet.Filter.Fields["customer"];
                int customerId;
                if (Int32.TryParse(filterValue, out customerId))
                    source = source.Where(c => c.CustomerId == customerId);
            }

            return source;
        }

        public static IQueryable<Contact> Include(this IQueryable<Contact> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Contact, object>>>();

            selectors.Add("country", c => c.Country);
            selectors.Add("language", c => c.Language);
            selectors.Add("honorific-prefix", c => c.HonorificPrefix);
            selectors.Add("customer", c => c.Customer);
            selectors.Add("telephones", c => c.Telephones);

            return source.Include<Contact>(querySet, selectors);
        }

        public static IQueryable<Contact> Sort(this IQueryable<Contact> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Contact, object>>>();

            selectors.Add("name", x => x.SearchName);
            selectors.Add("address", x => x.Address1);
            selectors.Add("postal-code", x => x.EmbeddedPostalCode);
            selectors.Add("city", x => x.EmbeddedCity);
            selectors.Add("created", x => x.Created);

            return source.Sort<Contact>(querySet, selectors);
        }

        public static IQueryable<Building> Filter(this IQueryable<Building> source, QuerySet querySet)
        {
            source = source.Filter<Building>(querySet);

            if (querySet.Filter.Fields.ContainsKey("number"))
            {
                var filterValue = querySet.Filter.Fields["number"];
                int number;
                if (Int32.TryParse(filterValue, out number))
                    source = source.Where(c => c.Number == number);
            }

            if (querySet.Filter.Fields.ContainsKey("customer"))
            {
                var filterValue = querySet.Filter.Fields["customer"];
                int customerId;
                if (Int32.TryParse(filterValue, out customerId))
                    source = source.Where(c => c.CustomerId == customerId);
            }

            return source;
        }

        public static IQueryable<Building> Include(this IQueryable<Building> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Building, object>>>();

            selectors.Add("country", c => c.Country);
            selectors.Add("language", c => c.Language);
            selectors.Add("honorific-prefix", c => c.HonorificPrefix);
            selectors.Add("customer", c => c.Customer);
            selectors.Add("telephones", c => c.Telephones);

            return source.Include<Building>(querySet, selectors);
        }

        public static IQueryable<Building> Sort(this IQueryable<Building> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Building, object>>>();

            selectors.Add("name", x => x.SearchName);
            selectors.Add("address", x => x.Address1);
            selectors.Add("postal-code", x => x.EmbeddedPostalCode);
            selectors.Add("city", x => x.EmbeddedCity);
            selectors.Add("created", x => x.Created);

            return source.Sort<Building>(querySet, selectors);
        }

        private static IQueryable<T> Filter<T>(this IQueryable<T> source, QuerySet querySet) where T : CustomerRecord
        {
            if (querySet.Filter.Fields.ContainsKey("name"))
            {
                var filterValue = querySet.Filter.Fields["name"].TextSearch();
                source = source.Where(c => EF.Functions.Like(c.SearchName, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("postal-code"))
            {
                var filterValue = querySet.Filter.Fields["postal-code"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.EmbeddedPostalCode, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("city"))
            {
                var filterValue = querySet.Filter.Fields["city"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.EmbeddedCity, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("street"))
            {
                var filterValue = querySet.Filter.Fields["street"].FilterWildcard();
                source = source.Where(c => EF.Functions.Like(c.Address1, filterValue)
                                        || EF.Functions.Like(c.Address2, filterValue)
                                        || EF.Functions.Like(c.Address3, filterValue));
            }

            if (querySet.Filter.Fields.ContainsKey("telephone"))
            {
                var search = querySet.Filter.Fields["telephone"];
                var predicate = search.ConstructTelephoneQuery();

                source = source.AsExpandable().Where(c => c.Telephones.Any(t => predicate.Invoke(t)));
            }

            if (querySet.Filter.Fields.ContainsKey("ids"))
            {
                var ids = querySet.Filter.Fields["ids"].Split(",").Select(int.Parse).ToList();
                source = source.Where(c => ids.Contains(c.DataId));
            }

            return source;
        }
    }
}