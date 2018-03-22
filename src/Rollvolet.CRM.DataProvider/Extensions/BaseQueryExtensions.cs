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
            Expression<Func<T, object>> selector;

            var field = querySet.Sort.Field;
            if (field != null && selectors.TryGetValue(field, out selector))
            {
                source = querySet.Sort.Order == SortQuery.ORDER_ASC ? source.OrderBy(selector) : source.OrderByDescending(selector);
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