using System.Collections.Generic;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Collectors
{
    public interface IIncludedCollector
    {
        IEnumerable<IResource> CollectIncluded(Customer customer, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(IEnumerable<Customer> customers, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(Contact contact, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(IEnumerable<Contact> contacts, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(Building building, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(IEnumerable<Building> buildings, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(Telephone telephone, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(IEnumerable<Telephone> telephones, IncludeQuery includeQuery);
    }
}