using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.DTO.Buildings;
using Rollvolet.CRM.APIContracts.DTO.Contacts;
using Rollvolet.CRM.APIContracts.DTO.Customers;
using Rollvolet.CRM.APIContracts.DTO.Deposits;
using Rollvolet.CRM.APIContracts.DTO.Invoices;
using Rollvolet.CRM.APIContracts.DTO.DepositInvoices;
using Rollvolet.CRM.APIContracts.DTO.Offers;
using Rollvolet.CRM.APIContracts.DTO.Orders;
using Rollvolet.CRM.APIContracts.DTO.Requests;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.APIContracts.DTO.Interventions;

namespace Rollvolet.CRM.API.Collectors
{
    public class IncludedCollector : IIncludedCollector
    {
        protected readonly IMapper _mapper;

        public IncludedCollector(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IEnumerable<IResource> CollectIncluded(Customer customer, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            // one-relations
            if (includeQuery.Contains("country") && customer.Country != null)
                included.Add(_mapper.Map<CountryDto>(customer.Country));
            if (includeQuery.Contains("language") && customer.Language != null)
                included.Add(_mapper.Map<LanguageDto>(customer.Language));
            if (includeQuery.Contains("honorific-prefix") && customer.HonorificPrefix != null)
                included.Add(_mapper.Map<HonorificPrefixDto>(customer.HonorificPrefix));

            // many-relations
            if (includeQuery.Contains("contacts") && customer.Contacts.Count() > 0)
                included.UnionWith(_mapper.Map<IEnumerable<ContactDto>>(customer.Contacts));
            if (includeQuery.Contains("buildings") && customer.Buildings.Count() > 0)
                included.UnionWith(_mapper.Map<IEnumerable<BuildingDto>>(customer.Buildings));
            if (includeQuery.Contains("tags") && customer.Tags.Count() > 0)
                included.UnionWith(_mapper.Map<IEnumerable<TagDto>>(customer.Tags));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Customer> customers, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            foreach (var customer in customers)
                included.UnionWith(CollectIncluded(customer, includeQuery));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(Contact contact, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            // one-relations
            if (includeQuery.Contains("country") && contact.Country != null)
                included.Add(_mapper.Map<CountryDto>(contact.Country));
            if (includeQuery.Contains("language") && contact.Language != null)
                included.Add(_mapper.Map<LanguageDto>(contact.Language));
            if (includeQuery.Contains("honorific-prefix") && contact.HonorificPrefix != null)
                included.Add(_mapper.Map<HonorificPrefixDto>(contact.HonorificPrefix));
            if (includeQuery.Contains("customer") && contact.Customer != null)
                included.Add(_mapper.Map<CustomerDto>(contact.Customer));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Contact> contacts, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            foreach (var contact in contacts)
                included.UnionWith(CollectIncluded(contact, includeQuery));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(Building building, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            // one-relations
            if (includeQuery.Contains("country") && building.Country != null)
                included.Add(_mapper.Map<CountryDto>(building.Country));
            if (includeQuery.Contains("language") && building.Language != null)
                included.Add(_mapper.Map<LanguageDto>(building.Language));
            if (includeQuery.Contains("honorific-prefix") && building.HonorificPrefix != null)
                included.Add(_mapper.Map<HonorificPrefixDto>(building.HonorificPrefix));
            if (includeQuery.Contains("customer") && building.Customer != null)
                included.Add(_mapper.Map<CustomerDto>(building.Customer));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Building> buildings, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            foreach (var building in buildings)
                included.UnionWith(CollectIncluded(building, includeQuery));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(Request request, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            var customerIncludeQuery = includeQuery.NestedInclude("customer");

            // one-relations
            if (includeQuery.Contains("customer") && request.Customer != null)
                included.Add(_mapper.Map<CustomerDto>(request.Customer, opt => opt.Items["include"] = customerIncludeQuery));
            if (includeQuery.Contains("customer.honorific-prefix") && request.Customer != null && request.Customer.HonorificPrefix != null)
                included.Add(_mapper.Map<HonorificPrefixDto>(request.Customer.HonorificPrefix));
            if (includeQuery.Contains("contact") && request.Contact != null)
                included.Add(_mapper.Map<ContactDto>(request.Contact));
            if (includeQuery.Contains("building") && request.Building != null)
                included.Add(_mapper.Map<BuildingDto>(request.Building));
            if (includeQuery.Contains("way-of-entry") && request.WayOfEntry != null)
                included.Add(_mapper.Map<WayOfEntryDto>(request.WayOfEntry));
            if (includeQuery.Contains("offer") && request.Offer != null)
                included.Add(_mapper.Map<OfferDto>(request.Offer));
            if (includeQuery.Contains("origin") && request.Origin != null)
                included.Add(_mapper.Map<InterventionDto>(request.Origin));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Request> requests, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            foreach (var request in requests)
                included.UnionWith(CollectIncluded(request, includeQuery));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(Intervention intervention, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            var customerIncludeQuery = includeQuery.NestedInclude("customer");

            // one-relations
            if (includeQuery.Contains("customer") && intervention.Customer != null)
                included.Add(_mapper.Map<CustomerDto>(intervention.Customer, opt => opt.Items["include"] = customerIncludeQuery));
            if (includeQuery.Contains("customer.honorific-prefix") && intervention.Customer != null && intervention.Customer.HonorificPrefix != null)
                included.Add(_mapper.Map<HonorificPrefixDto>(intervention.Customer.HonorificPrefix));
            if (includeQuery.Contains("contact") && intervention.Contact != null)
                included.Add(_mapper.Map<ContactDto>(intervention.Contact));
            if (includeQuery.Contains("building") && intervention.Building != null)
                included.Add(_mapper.Map<BuildingDto>(intervention.Building));
            if (includeQuery.Contains("way-of-entry") && intervention.WayOfEntry != null)
                included.Add(_mapper.Map<WayOfEntryDto>(intervention.WayOfEntry));
            if (includeQuery.Contains("invoice") && intervention.Invoice != null)
                included.Add(_mapper.Map<InvoiceDto>(intervention.Invoice));
            if (includeQuery.Contains("follow-up-request") && intervention.FollowUpRequest != null)
                included.Add(_mapper.Map<RequestDto>(intervention.FollowUpRequest));
            if (includeQuery.Contains("origin") && intervention.Origin != null)
                included.Add(_mapper.Map<OrderDto>(intervention.Origin));

            // many-relations
            if (includeQuery.Contains("technicians") && intervention.Technicians.Count() > 0)
                included.UnionWith(_mapper.Map<IEnumerable<EmployeeDto>>(intervention.Technicians));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Intervention> interventions, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            foreach (var intervention in interventions)
                included.UnionWith(CollectIncluded(intervention, includeQuery));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(Offer offer, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            var customerIncludeQuery = includeQuery.NestedInclude("customer");
            var requestIncludeQuery = includeQuery.NestedInclude("request");

            // one-relations
            if (includeQuery.Contains("order") && offer.Order != null)
                included.Add(_mapper.Map<OrderDto>(offer.Order));
            if (includeQuery.Contains("customer") && offer.Customer != null)
                included.Add(_mapper.Map<CustomerDto>(offer.Customer, opt => opt.Items["include"] = customerIncludeQuery));
            if (includeQuery.Contains("customer.honorific-prefix") && offer.Customer != null && offer.Customer.HonorificPrefix != null)
                included.Add(_mapper.Map<HonorificPrefixDto>(offer.Customer.HonorificPrefix));
            if (includeQuery.Contains("contact") && offer.Contact != null)
                included.Add(_mapper.Map<ContactDto>(offer.Contact));
            if (includeQuery.Contains("building") && offer.Building != null)
                included.Add(_mapper.Map<BuildingDto>(offer.Building));
            if (includeQuery.Contains("vat-rate") && offer.VatRate != null)
                included.Add(_mapper.Map<VatRateDto>(offer.VatRate));
            if (includeQuery.Contains("request") && offer.Request != null)
                included.Add(_mapper.Map<RequestDto>(offer.Request, opt => opt.Items["include"] = requestIncludeQuery));
            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Offer> offers, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            foreach (var offer in offers)
                included.UnionWith(CollectIncluded(offer, includeQuery));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(Order order, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            var customerIncludeQuery = includeQuery.NestedInclude("customer");

            // one-relations
            if (includeQuery.Contains("offer") && order.Offer != null)
                included.Add(_mapper.Map<OfferDto>(order.Offer));
            if (includeQuery.Contains("invoice") && order.Invoice != null)
                included.Add(_mapper.Map<InvoiceDto>(order.Invoice));
            if (includeQuery.Contains("customer") && order.Customer != null)
                included.Add(_mapper.Map<CustomerDto>(order.Customer, opt => opt.Items["include"] = customerIncludeQuery));
            if (includeQuery.Contains("customer.honorific-prefix") && order.Customer != null && order.Customer.HonorificPrefix != null)
                included.Add(_mapper.Map<HonorificPrefixDto>(order.Customer.HonorificPrefix));
            if (includeQuery.Contains("contact") && order.Contact != null)
                included.Add(_mapper.Map<ContactDto>(order.Contact));
            if (includeQuery.Contains("building") && order.Building != null)
                included.Add(_mapper.Map<BuildingDto>(order.Building));
            if (includeQuery.Contains("vat-rate") && order.VatRate != null)
                included.Add(_mapper.Map<VatRateDto>(order.VatRate));

            // many-relations
            if (includeQuery.Contains("deposits") && order.Deposits.Count() > 0)
                included.UnionWith(_mapper.Map<IEnumerable<DepositDto>>(order.Deposits));
            if (includeQuery.Contains("deposit-invoices") && order.DepositInvoices.Count() > 0)
                included.UnionWith(_mapper.Map<IEnumerable<DepositInvoiceDto>>(order.DepositInvoices));
            if (includeQuery.Contains("interventions") && order.Interventions.Count() > 0)
                included.UnionWith(_mapper.Map<IEnumerable<InterventionDto>>(order.Interventions));
            if (includeQuery.Contains("technicians") && order.Technicians.Count() > 0)
                included.UnionWith(_mapper.Map<IEnumerable<EmployeeDto>>(order.Technicians));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Order> orders, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            foreach (var order in orders)
                included.UnionWith(CollectIncluded(order, includeQuery));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(Invoice invoice, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            var customerIncludeQuery = includeQuery.NestedInclude("customer");

            // one-relations
            if (includeQuery.Contains("order") && invoice.Order != null)
                included.Add(_mapper.Map<OrderDto>(invoice.Order));
            if (includeQuery.Contains("customer") && invoice.Customer != null)
                included.Add(_mapper.Map<CustomerDto>(invoice.Customer, opt => opt.Items["include"] = customerIncludeQuery));
            if (includeQuery.Contains("customer.honorific-prefix") && invoice.Customer != null && invoice.Customer.HonorificPrefix != null)
                included.Add(_mapper.Map<HonorificPrefixDto>(invoice.Customer.HonorificPrefix));
            if (includeQuery.Contains("contact") && invoice.Contact != null)
                included.Add(_mapper.Map<ContactDto>(invoice.Contact));
            if (includeQuery.Contains("building") && invoice.Building != null)
                included.Add(_mapper.Map<BuildingDto>(invoice.Building));
            if (includeQuery.Contains("vat-rate") && invoice.VatRate != null)
                included.Add(_mapper.Map<VatRateDto>(invoice.VatRate));

            // many-relations
            if (includeQuery.Contains("deposits") && invoice.Deposits.Count() > 0)
                included.UnionWith(_mapper.Map<IEnumerable<DepositDto>>(invoice.Deposits));
            if (includeQuery.Contains("deposit-invoices") && invoice.DepositInvoices.Count() > 0)
                included.UnionWith(_mapper.Map<IEnumerable<DepositInvoiceDto>>(invoice.DepositInvoices));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Invoice> invoices, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            foreach (var invoice in invoices)
                included.UnionWith(CollectIncluded(invoice, includeQuery));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(Tag tag, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Tag> tags, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            foreach (var tag in tags)
                included.UnionWith(CollectIncluded(tag, includeQuery));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(Employee employee, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Employee> employees, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            foreach (var employee in employees)
                included.UnionWith(CollectIncluded(employee, includeQuery));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(Deposit deposit, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            // one-relations
            if (includeQuery.Contains("customer") && deposit.Customer != null)
                included.Add(_mapper.Map<CustomerDto>(deposit.Customer));
            if (includeQuery.Contains("order") && deposit.Order != null)
                included.Add(_mapper.Map<OrderDto>(deposit.Order));
            if (includeQuery.Contains("invoice") && deposit.Invoice != null)
                included.Add(_mapper.Map<InvoiceDto>(deposit.Invoice));
            if (includeQuery.Contains("payment") && deposit.Payment != null)
                included.Add(_mapper.Map<PaymentDto>(deposit.Payment));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Deposit> deposits, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            foreach (var deposit in deposits)
                included.UnionWith(CollectIncluded(deposit, includeQuery));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(DepositInvoice depositInvoice, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            var customerIncludeQuery = includeQuery.NestedInclude("customer");

            // one-relations
            if (includeQuery.Contains("order") && depositInvoice.Order != null)
                included.Add(_mapper.Map<OrderDto>(depositInvoice.Order));
            if (includeQuery.Contains("customer") && depositInvoice.Customer != null)
                included.Add(_mapper.Map<CustomerDto>(depositInvoice.Customer, opt => opt.Items["include"] = customerIncludeQuery));
            if (includeQuery.Contains("customer.honorific-prefix") && depositInvoice.Customer != null && depositInvoice.Customer.HonorificPrefix != null)
                included.Add(_mapper.Map<HonorificPrefixDto>(depositInvoice.Customer.HonorificPrefix));
            if (includeQuery.Contains("contact") && depositInvoice.Contact != null)
                included.Add(_mapper.Map<ContactDto>(depositInvoice.Contact));
            if (includeQuery.Contains("building") && depositInvoice.Building != null)
                included.Add(_mapper.Map<BuildingDto>(depositInvoice.Building));
            if (includeQuery.Contains("vat-rate") && depositInvoice.VatRate != null)
                included.Add(_mapper.Map<VatRateDto>(depositInvoice.VatRate));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<DepositInvoice> depositInvoices, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();

            foreach (var invoice in depositInvoices)
                included.UnionWith(CollectIncluded(invoice, includeQuery));

            return included;
        }
    }

}