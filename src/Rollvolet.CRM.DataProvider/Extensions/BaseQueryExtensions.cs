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
    public static class BaseQueryExtensions
    {
        public static Expression<Func<Telephone, bool>> ConstructTelephoneQuery(this string search)
        {
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

            var predicate = PredicateBuilder.New<Telephone>(true);
            predicate.And(t =>
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
                ));

            return predicate;
        }

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