using System;
using Microsoft.AspNetCore.Http;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.Domain.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Rollvolet.CRM.API.Builders.Interfaces;

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
                    // TODO throw exception if System.IndexOutOfRangeException
                    var propertyName = pair.Key.Split('[', ']')[1];

                    if (propertyName == "size")
                        querySet.Page.Size = Convert.ToInt32(pair.Value);
                    else if (propertyName == "number")
                        querySet.Page.Number = Convert.ToInt32(pair.Value);
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
            }

            return querySet;
        }

        public Links BuildSingleResourceLinks(string path)
        {
            var links = new Links();

            links.Self = $"{path}";

            return links;
        }

        public Links BuildLinks(string path)
        {
            var links = new Links();

            links.Self = $"{path}";

            return links;
        }

        public Links BuildLinks(string path, string id)
        {
            var links = new Links();

            links.Self = $"{path}/{id}";

            return links;
        }

        public CollectionLinks BuildLinks(string path, QuerySet query, IPaged paged)
        {
            var links = new CollectionLinks();

            links.Self = $"{path}?{BuildPaginationQuery(paged.PageSize, paged.PageNumber)}{BuildIncludeQuery(query.Include.Fields)}{BuildSortQuery(query.Sort.Field, query.Sort.Order)}";
            links.First = $"{path}?{BuildPaginationQuery(paged.PageSize, paged.First)}{BuildIncludeQuery(query.Include.Fields)}{BuildSortQuery(query.Sort.Field, query.Sort.Order)}";
            links.Last = $"{path}?{BuildPaginationQuery(paged.PageSize, paged.Last)}{BuildIncludeQuery(query.Include.Fields)}{BuildSortQuery(query.Sort.Field, query.Sort.Order)}";
            
            if (paged.HasPrev)
                links.Prev = $"{path}?{BuildPaginationQuery(paged.PageSize, paged.Prev)}{BuildIncludeQuery(query.Include.Fields)}{BuildSortQuery(query.Sort.Field, query.Sort.Order)}";
            if (paged.HasNext)
                links.Next = $"{path}?{BuildPaginationQuery(paged.PageSize, paged.Next)}{BuildIncludeQuery(query.Include.Fields)}{BuildSortQuery(query.Sort.Field, query.Sort.Order)}";

            return links;
        }

        public object BuildMeta(IPaged paged)
        {
            return new { Count = paged.Count, Page = new { Number = paged.PageNumber, Size = paged.PageSize } };
        }

        private string BuildPaginationQuery(int size, int number)
        {
            return $"page[size]={size}&page[number]={number}";
        }

        private string BuildIncludeQuery(string[] fields)
        {
            return fields.Length > 0 ? $"&include={string.Join(",", fields)}" : "";
        }

        private string BuildSortQuery(string field, string order)
        {
            if (!string.IsNullOrEmpty(field))
            {
                return order == SortQuery.ORDER_ASC ? $"&sort={field}" : $"&sort=-{field}";
            }
            else
            {
                return "";
            }
        }
    }
}