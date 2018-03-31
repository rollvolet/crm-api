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
        IEnumerable<IResource> CollectIncluded(Request request, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(IEnumerable<Request> requests, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(Offer offer, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(IEnumerable<Offer> offers, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(Order order, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(IEnumerable<Order> orders, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(Invoice invoice, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(IEnumerable<Invoice> invoices, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(Tag tag, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(IEnumerable<Tag> tags, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(Deposit deposit, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(IEnumerable<Deposit> deposits, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(DepositInvoice depositInvoice, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(IEnumerable<DepositInvoice> depositInvoices, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(WorkingHour workingHour, IncludeQuery includeQuery);
        IEnumerable<IResource> CollectIncluded(IEnumerable<WorkingHour> workingHours, IncludeQuery includeQuery);
    }
}