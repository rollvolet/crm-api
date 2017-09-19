using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
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
            selectors.Add("postal-code", c => c.PostalCode);
            selectors.Add("honorific-prefix", c => c.HonorificPrefix);
            selectors.Add("contacts", c => c.Contacts);
            selectors.Add("buildings", c => c.Buildings);
            selectors.Add("telephones", c => c.Telephones);

            return Include<Customer>(source, querySet, selectors);              
        }

        public static IQueryable<Customer> Sort(this IQueryable<Customer> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Customer, string>>>();
            
            selectors.Add("name", x => x.Name);

            return Sort<Customer>(source, querySet, selectors);
        }

        public static IQueryable<Contact> Include(this IQueryable<Contact> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Contact, object>>>();
            
            selectors.Add("country", c => c.Country);
            selectors.Add("language", c => c.Language);
            selectors.Add("postal-code", c => c.PostalCode);

            return Include<Contact>(source, querySet, selectors);         
        }

        public static IQueryable<Contact> Sort(this IQueryable<Contact> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Contact, string>>>();
            
            selectors.Add("name", x => x.Name);

            return Sort<Contact>(source, querySet, selectors);
        }

        public static IQueryable<Building> Include(this IQueryable<Building> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Building, object>>>();
            
            selectors.Add("country", c => c.Country);
            selectors.Add("language", c => c.Language);
            selectors.Add("postal-code", c => c.PostalCode);

            return Include<Building>(source, querySet, selectors);         
        }

        public static IQueryable<Building> Sort(this IQueryable<Building> source, QuerySet querySet)
        {
            var selectors = new Dictionary<string, Expression<Func<Building, string>>>();
            
            selectors.Add("name", x => x.Name);

            return Sort<Building>(source, querySet, selectors);
        }

        private static IQueryable<T> Include<T>(IQueryable<T> source, QuerySet querySet, IDictionary<string, Expression<Func<T, object>>> selectors) where T : class
        {
            foreach(var field in querySet.Include.Fields)
            {
                var selector = selectors[field];
            
                if (selector != null)
                {
                    source = source.Include(selector);
                }
            }

            return source;
        }        

        private static IQueryable<T> Sort<T>(IQueryable<T> source, QuerySet querySet, IDictionary<string, Expression<Func<T, string>>> selectors)
        {
            var selector = querySet.Sort.Field != null ? selectors[querySet.Sort.Field] : null;

            if (selector != null)
            {
                source = querySet.Sort.Order == SortQuery.ORDER_ASC ? source.OrderBy(selector) : source.OrderByDescending(selector);
            }

            return source;
        }
    }
}