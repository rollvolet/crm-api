using System;
using Microsoft.AspNetCore.Http;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.Domain.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Rollvolet.CRM.API.Builders.Interfaces;
using System.Text.RegularExpressions;
using Rollvolet.CRM.Domain.Exceptions;

namespace Rollvolet.CRM.API.Builders
{
    public class JsonApiBuilder : IJsonApiBuilder
    {
        public QuerySet BuildQuerySet(IQueryCollection query)
        {
            var querySet = new QuerySet();

            foreach (var pair in query)
            {
                if (pair.Key.StartsWith("page"))
                {
                    try
                    {
                        var propertyName = pair.Key.Split('[', ']')[1];

                        if (propertyName == "size")
                            querySet.Page.Size = Convert.ToInt32(pair.Value);
                        else if (propertyName == "number")
                            querySet.Page.Number = Convert.ToInt32(pair.Value);
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        throw new IllegalArgumentException("IllegalPage", "Page query parameter must have key 'page[size]' or 'page[number]'.");
                    }
                    catch (System.FormatException)
                    {
                        throw new IllegalArgumentException("IllegalPage", "Page query parameter value must be an integer.");
                    }
                }

                if (pair.Key.StartsWith("include"))
                {
                    var includeFields = pair.Value.ToString().Split(',');

                    querySet.Include.Fields = includeFields;
                }

                if (pair.Key.StartsWith("sort"))
                {
                    var field = pair.Value.ToString();

                    if (field.StartsWith("-"))
                    {
                        querySet.Sort.Order = SortQuery.ORDER_DESC;
                        querySet.Sort.Field = field.Substring(1);
                    }
                    else
                    {
                        querySet.Sort.Order = SortQuery.ORDER_ASC;
                        querySet.Sort.Field = field;
                    }
                }

                if (pair.Key.StartsWith("filter"))
                {
                    var propertyPath = GeneratePropertyPath(pair.Key);
                    querySet.Filter.Fields.Add(propertyPath, pair.Value);
                }
            }

            return querySet;
        }

        private string GeneratePropertyPath(string key)
        {
            var pattern = @"\[(.*?)\]";
            var propertySegments = Regex.Matches(key, pattern).Select(x => x.Groups[1]);
            return String.Join(".", propertySegments);
        }

        public Links BuildSingleResourceLinks(string path, QuerySet query)
        {
            var links = new Links();
            
            var includeQuery = BuildIncludeQuery(query.Include.Fields);

            links.Self = String.IsNullOrEmpty(includeQuery) ? path : $"{path}?{includeQuery}";
            
            return links;
        }

        public Links BuildNewSingleResourceLinks(string path, string id)
        {
            var links = new Links();

            links.Self = $"{path}/{id}";

            return links;
        }

        public CollectionLinks BuildCollectionLinks(string path, QuerySet query, IPaged paged)
        {
            var links = new CollectionLinks();

            var includeQuery = BuildIncludeQuery(query.Include.Fields);
            var sortQuery = BuildSortQuery(query.Sort.Field, query.Sort.Order);

            links.Self = $"{path}?{String.Join("&", new List<string>() { BuildPaginationQuery(paged.PageSize, paged.PageNumber), includeQuery, sortQuery }.FindAll(q => q != null))}";
            links.First = $"{path}?{String.Join("&", new List<string>() { BuildPaginationQuery(paged.PageSize, paged.First), includeQuery, sortQuery }.FindAll(q => q != null))}";
            links.Last = $"{path}?{String.Join("&", new List<string>() { BuildPaginationQuery(paged.PageSize, paged.Last), includeQuery, sortQuery }.FindAll(q => q != null))}";
            
            if (paged.HasPrev)
                links.Prev = $"{path}?{String.Join("&", new List<string>() { BuildPaginationQuery(paged.PageSize, paged.Prev), includeQuery, sortQuery }.FindAll(q => q != null))}";
            if (paged.HasNext)
                links.Next = $"{path}?{String.Join("&", new List<string>() { BuildPaginationQuery(paged.PageSize, paged.Next), includeQuery, sortQuery }.FindAll(q => q != null))}";

            return links;
        }

        public object BuildCollectionMetadata(IPaged paged)
        {
            return new { Count = paged.Count, Page = new { Number = paged.PageNumber, Size = paged.PageSize } };
        }

        private string BuildPaginationQuery(int size, int number)
        {
            return $"page[size]={size}&page[number]={number}";
        }

        private string BuildIncludeQuery(string[] fields)
        {
            return fields.Length > 0 ? $"include={string.Join(",", fields)}" : null;
        }

        private string BuildSortQuery(string field, string order)
        {
            if (!string.IsNullOrEmpty(field))
            {
                return order == SortQuery.ORDER_ASC ? $"sort={field}" : $"sort=-{field}";
            }
            else
            {
                return null;
            }
        }
    }
}