using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProvider.Extensions
{
    public static class QueryExtensions
    {
        public static IQueryable<Customer> Include(this IQueryable<Customer> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Customer, object>>>();
            
            selectors.Add("country", c => c.Country);
            selectors.Add("language", c => c.Language);
            selectors.Add("honorific-prefix", c => c.HonorificPrefix);
            selectors.Add("contacts", c => c.Contacts);
            selectors.Add("buildings", c => c.Buildings);
            selectors.Add("telephones", c => c.Telephones);

            return Include<Customer>(source, querySet, selectors);              
        }

        public static IQueryable<Customer> Sort(this IQueryable<Customer> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Customer, object>>>();
            
            selectors.Add("name", x => x.SearchName);
            selectors.Add("number", x => x.Number);
            selectors.Add("postal-code", x => x.EmbeddedPostalCode);
            selectors.Add("city", x => x.EmbeddedCity);
            selectors.Add("created", x => x.Created);
            selectors.Add("updated", x => x.Updated);

            return Sort<Customer>(source, querySet, selectors);
        }

        public static IQueryable<Contact> Include(this IQueryable<Contact> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Contact, object>>>();
            
            selectors.Add("country", c => c.Country);
            selectors.Add("language", c => c.Language);

            return Include<Contact>(source, querySet, selectors);         
        }

        public static IQueryable<Contact> Sort(this IQueryable<Contact> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Contact, object>>>();
            
            selectors.Add("name", x => x.SearchName);
            selectors.Add("postal-code", x => x.EmbeddedPostalCode);
            selectors.Add("city", x => x.EmbeddedCity);
            selectors.Add("created", x => x.Created);
            selectors.Add("updated", x => x.Updated);

            return Sort<Contact>(source, querySet, selectors);
        }

        public static IQueryable<Building> Include(this IQueryable<Building> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Building, object>>>();
            
            selectors.Add("country", c => c.Country);
            selectors.Add("language", c => c.Language);

            return Include<Building>(source, querySet, selectors);         
        }

        public static IQueryable<Building> Sort(this IQueryable<Building> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Building, object>>>();
            
            selectors.Add("name", x => x.SearchName);
            selectors.Add("postal-code", x => x.EmbeddedPostalCode);
            selectors.Add("city", x => x.EmbeddedCity);
            selectors.Add("created", x => x.Created);
            selectors.Add("updated", x => x.Updated);

            return Sort<Building>(source, querySet, selectors);
        }

        public static IQueryable<Telephone> Include(this IQueryable<Telephone> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Telephone, object>>>();
            
            selectors.Add("country", c => c.Country);
            selectors.Add("telephone-type", c => c.TelephoneType);

            return Include<Telephone>(source, querySet, selectors);         
        }

        public static IQueryable<Telephone> Sort(this IQueryable<Telephone> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Telephone, object>>>();
            
            selectors.Add("order", x => x.Order.ToString());

            return Sort<Telephone>(source, querySet, selectors);
        }

        private static IQueryable<T> Include<T>(IQueryable<T> source, QuerySet querySet, IDictionary<string, Expression<Func<T, object>>> selectors) where T : class
        {
            foreach(var field in querySet.Include.Fields)
            {
                Expression<Func<T, object>> selector;
            
                if (selectors.TryGetValue(field, out selector))
                {
                    source = source.Include(selector);
                }
                // TODO: add else-clause to log invalid include field
            }

            return source;
        }        

        private static IQueryable<T> Sort<T>(IQueryable<T> source, QuerySet querySet, IDictionary<string, Expression<Func<T, object>>> selectors)
        {
            Expression<Func<T, object>> selector;

            if (querySet.Sort.Field != null && selectors.TryGetValue(querySet.Sort.Field, out selector))
            {
                source = querySet.Sort.Order == SortQuery.ORDER_ASC ? source.OrderBy(selector) : source.OrderByDescending(selector);
            }
            // TODO: add else-clause to log invalid sort field

            return source;
        }
    }
}