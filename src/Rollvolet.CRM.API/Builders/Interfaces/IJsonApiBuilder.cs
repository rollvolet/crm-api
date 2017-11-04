using System;
using Microsoft.AspNetCore.Http;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.Domain.Models.Interfaces;

namespace Rollvolet.CRM.API.Builders.Interfaces
{
    public interface IJsonApiBuilder
    {
        QuerySet BuildQuerySet(IQueryCollection query);
        Links BuildSingleResourceLinks(string path);
        Links BuildLinks(string path);
        Links BuildLinks(string path, string id);
        CollectionLinks BuildLinks(string path, QuerySet query, IPaged paged);
        object BuildMeta(IPaged paged);
    }
}