using Microsoft.AspNetCore.Http;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.Domain.Models.Interfaces;

namespace Rollvolet.CRM.API.Builders.Interfaces
{
    public interface IJsonApiBuilder
    {
        QuerySet BuildQuerySet(IQueryCollection query);
        Links BuildSingleResourceLinks(string path, QuerySet query = null);
        Links BuildNewSingleResourceLinks(string path, string id);
        CollectionLinks BuildCollectionLinks(string path, QuerySet query, IPaged paged);
        object BuildCollectionMetadata(IPaged paged);
    }
}