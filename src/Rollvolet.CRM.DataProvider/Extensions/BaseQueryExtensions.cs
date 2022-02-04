using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.Domain.Logging;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProvider.Extensions
{
    public static class BaseQueryExtensions
    {
        public static string FilterWhitespace(this string value)
        {
            return Regex.Replace(value, @"\s+", "");
        }

        // see https://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
        public static string FilterDiacritics(this string value)
        {
            var normalizedString = value.Normalize(NormalizationForm.FormD);

            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string FilterWildcard(this string value)
        {
            return value.Replace("*", "%") + "%";
        }

        public static string TextSearch(this string value)
        {
            return value.FilterWhitespace().FilterWildcard().FilterDiacritics();
        }

        public static IQueryable<T> ForPage<T>(this IQueryable<T> source, QuerySet querySet)
        {
            if (querySet.Page.IsPaged)
                return source.Skip(querySet.Page.Skip).Take(querySet.Page.Take);
            else
                return source;
        }

        public static IQueryable<T> Include<T>(this IQueryable<T> source, QuerySet querySet, IDictionary<string, Expression<Func<T, object>>> selectors) where T : class
        {
            foreach(var field in querySet.Include.Fields)
            {
                Expression<Func<T, object>> selector;

                var includesSelector = selectors.TryGetValue(field, out selector);
                if (includesSelector)
                {
                    if (selector != null)
                        source = source.Include(selector);
                }
                else
                {
                    ApplicationLogging.CreateLogger(typeof(BaseQueryExtensions).AssemblyQualifiedName).
                        LogWarning("'{Value}' is not supported as include value on type {Type}", field, typeof(T).AssemblyQualifiedName);
                }
            }

            return source;
        }

        public static IQueryable<T> Sort<T>(this IQueryable<T> source, QuerySet querySet, IDictionary<string, Expression<Func<T, object>>> selectors)
        {
            var multiFieldSelectors = new Dictionary<string, IEnumerable<Expression<Func<T, object>>>>();

            foreach (KeyValuePair<string, Expression<Func<T, object>>> kvp in selectors)
            {
                var listValue = new List<Expression<Func<T, object>>> { kvp.Value };
                multiFieldSelectors.Add(kvp.Key, listValue);
            }

            return source.Sort(querySet, multiFieldSelectors);
        }

        public static IQueryable<T> Sort<T>(this IQueryable<T> source, QuerySet querySet, IDictionary<string, IEnumerable<Expression<Func<T, object>>>> selectors)
        {
            // TODO add better support to filter on multiple fields.
            // Now we assume all fields are sorted ASC or DESC. But actually querySet.Sort should support multiple fields
            IEnumerable<Expression<Func<T, object>>> selectorsForField;

            var field = querySet.Sort.Field;
            if (field != null && selectors.TryGetValue(field, out selectorsForField))
            {
                int i = 0;
                foreach (Expression<Func<T, object>> selector in selectorsForField)
                {
                    if (i == 0)
                    {
                        source = querySet.Sort.IsAscending ? source.OrderBy(selector) : source.OrderByDescending(selector);
                    }
                    else
                    {
                        var orderedQueryable = (IOrderedQueryable<T>) source;
                        source = querySet.Sort.IsAscending ? orderedQueryable.ThenBy(selector) : orderedQueryable.ThenByDescending(selector);
                    }
                    i++;
                }
                            }
            else if (field != null)
            {
                ApplicationLogging.CreateLogger(typeof(BaseQueryExtensions).AssemblyQualifiedName)
                    .LogWarning("'{Value}' is not supported as sort value on type {Type}", field, typeof(T).AssemblyQualifiedName);
            }

            return source;
        }
    }
}