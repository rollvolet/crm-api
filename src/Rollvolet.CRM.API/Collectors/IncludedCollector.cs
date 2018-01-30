using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

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
            if (includeQuery.Contains("telephones") && customer.Telephones.Count() > 0)
                included.UnionWith(_mapper.Map<IEnumerable<TelephoneDto>>(customer.Telephones));
            if (includeQuery.Contains("requests") && customer.Requests.Count() > 0)
                included.UnionWith(_mapper.Map<IEnumerable<RequestDto>>(customer.Requests));
            if (includeQuery.Contains("offers") && customer.Offers.Count() > 0)
                included.UnionWith(_mapper.Map<IEnumerable<OfferDto>>(customer.Offers));
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
                
            // many-relations
            if (includeQuery.Contains("telephones") && contact.Telephones.Count() > 0)
                included.UnionWith(_mapper.Map<IEnumerable<TelephoneDto>>(contact.Telephones));

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
                
            // many-relations
            if (includeQuery.Contains("telephones") && building.Telephones.Count() > 0)
                included.UnionWith(_mapper.Map<IEnumerable<TelephoneDto>>(building.Telephones));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Building> buildings, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();
            
            foreach (var building in buildings)
                included.UnionWith(CollectIncluded(building, includeQuery));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(Telephone telephone, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();
            
            if (includeQuery.Contains("country") && telephone.Country != null)
                included.Add(_mapper.Map<CountryDto>(telephone.Country));
            if (includeQuery.Contains("telephone-type") && telephone.TelephoneType != null)
                included.Add(_mapper.Map<TelephoneTypeDto>(telephone.TelephoneType));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Telephone> telephones, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();
            
            foreach (var telephone in telephones)
                included.UnionWith(CollectIncluded(telephone, includeQuery));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(Request request, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();
            
            // one-relations
            if (includeQuery.Contains("customer") && request.Customer != null)
                included.Add(_mapper.Map<CustomerDto>(request.Customer));
            if (includeQuery.Contains("contact") && request.Contact != null)
                included.Add(_mapper.Map<ContactDto>(request.Contact));
            if (includeQuery.Contains("building") && request.Building != null)
                included.Add(_mapper.Map<BuildingDto>(request.Building));
            if (includeQuery.Contains("way-of-entry") && request.WayOfEntry != null)
                included.Add(_mapper.Map<WayOfEntryDto>(request.WayOfEntry));
            if (includeQuery.Contains("visit") && request.Visit != null)
                included.Add(_mapper.Map<VisitDto>(request.Visit));
            if (includeQuery.Contains("offer") && request.Offer != null)
                included.Add(_mapper.Map<OfferDto>(request.Offer));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Request> requests, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();
            
            foreach (var request in requests)
                included.UnionWith(CollectIncluded(request, includeQuery));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(Offer offer, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();
            
            // one-relations
            if (includeQuery.Contains("request") && offer.Request != null)
                included.Add(_mapper.Map<RequestDto>(offer.Request));
            if (includeQuery.Contains("customer") && offer.Customer != null)
                included.Add(_mapper.Map<CustomerDto>(offer.Customer));
            if (includeQuery.Contains("contact") && offer.Contact != null)
                included.Add(_mapper.Map<ContactDto>(offer.Contact));
            if (includeQuery.Contains("building") && offer.Building != null)
                included.Add(_mapper.Map<BuildingDto>(offer.Building));
            if (includeQuery.Contains("vat-rate") && offer.VatRate != null)
                included.Add(_mapper.Map<VatRateDto>(offer.VatRate));
            if (includeQuery.Contains("submission-type") && offer.SubmissionType != null)
                included.Add(_mapper.Map<SubmissionTypeDto>(offer.SubmissionType));
            if (includeQuery.Contains("product") && offer.Product != null)
                included.Add(_mapper.Map<ProductDto>(offer.Product));

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
            
            // one-relations
            if (includeQuery.Contains("offer") && order.Offer != null)
                included.Add(_mapper.Map<OfferDto>(order.Offer));
            if (includeQuery.Contains("customer") && order.Customer != null)
                included.Add(_mapper.Map<CustomerDto>(order.Customer));
            if (includeQuery.Contains("contact") && order.Contact != null)
                included.Add(_mapper.Map<ContactDto>(order.Contact));
            if (includeQuery.Contains("building") && order.Building != null)
                included.Add(_mapper.Map<BuildingDto>(order.Building));

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Order> orders, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();
            
            foreach (var order in orders)
                included.UnionWith(CollectIncluded(order, includeQuery));

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
    }

}