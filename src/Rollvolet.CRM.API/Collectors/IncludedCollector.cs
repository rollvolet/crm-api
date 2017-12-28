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

            return included;
        }

        public IEnumerable<IResource> CollectIncluded(IEnumerable<Request> requests, IncludeQuery includeQuery)
        {
            ISet<IResource> included = new HashSet<IResource>();
            
            foreach (var request in requests)
                included.UnionWith(CollectIncluded(request, includeQuery));

            return included;
        }
    }

}