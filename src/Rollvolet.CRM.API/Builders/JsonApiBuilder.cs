using System;
using Microsoft.AspNetCore.Http;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Interfaces;

namespace Rollvolet.CRM.API.Builders
{
    public class JsonApiBuilder
    {
        public QuerySet BuildQuerySet(IQueryCollection query)
        {
            var querySet = new QuerySet();

            foreach (var pair in query)
            {
                if (pair.Key.StartsWith("page"))
                {
                    var propertyName = pair.Key.Split('[', ']')[1];

                    if (propertyName == "size")
                        querySet.Page.Size = Convert.ToInt32(pair.Value);
                    else if (propertyName == "number")
                        querySet.Page.Number = Convert.ToInt32(pair.Value);
                }
            }

            return querySet;
        }

        public Links BuildLinks(string path)
        {
            var links = new Links();

            links.Self = $"{path}";

            return links;
        }

        public CollectionLinks BuildLinks(string path, QuerySet query, IPaged paged)
        {
            var links = new CollectionLinks();

            links.Self = $"{path}?{this.BuildPaginationQuery(paged.PageSize, paged.PageNumber)}";
            links.First = $"{path}?{this.BuildPaginationQuery(paged.PageSize, paged.First)}";
            links.Last = $"{path}?{this.BuildPaginationQuery(paged.PageSize, paged.Last)}";
            
            if (paged.HasPrev)
                links.Prev = $"{path}?{this.BuildPaginationQuery(paged.PageSize, paged.Prev)}";
            if (paged.HasNext)
                links.Next = $"{path}?{this.BuildPaginationQuery(paged.PageSize, paged.Next)}";

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
    }
}