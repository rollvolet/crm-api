using System;
using Microsoft.AspNetCore.Http;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Interfaces;

namespace Rollvolet.CRM.API.Builders
{
    public interface IJsonApiBuilder
    {
        QuerySet BuildQuerySet(IQueryCollection query);
        Links BuildLinks(string path);
        CollectionLinks BuildLinks(string path, QuerySet query, IPaged paged);
        object BuildMeta(IPaged paged);
    }
}