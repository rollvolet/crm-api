using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProvider.Extensions
{
    public static class BaseQueryExtensions
    {
        public static string FilterWhitespace(this string value)
        {
            return Regex.Replace(value, @"\s+", "");
        }

        public static string FilterWildcard(this string value)
        {
            return value.Replace("*", "%") + "%";
        }

        public static IQueryable<T> Include<T>(this IQueryable<T> source, QuerySet querySet, IDictionary<string, Expression<Func<T, object>>> selectors) where T : class
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

        public static IQueryable<T> Sort<T>(this IQueryable<T> source, QuerySet querySet, IDictionary<string, Expression<Func<T, object>>> selectors)
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