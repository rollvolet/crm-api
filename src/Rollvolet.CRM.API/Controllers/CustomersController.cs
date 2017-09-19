using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.API.Builders;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    public class CustomersController : Controller
    {
        private readonly ICustomerManager _customerManager;
        private readonly IContactManager _contactManager;
        private readonly IBuildingManager _buildingManager;
        
        private readonly IMapper _mapper;

        public CustomersController(ICustomerManager customerManager, IContactManager contactManager, IBuildingManager buildingManager, IMapper mapper)
        {
            _customerManager = customerManager;
            _contactManager = contactManager;
            _buildingManager = buildingManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var jsonApiBuilder = new JsonApiBuilder();
            var querySet = jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedCustomers = await _customerManager.GetAllAsync(querySet);

            var included = new HashSet<Resource>();
            var customerResources = new List<CustomerDto>();

            foreach (var customer in pagedCustomers.Items)
            {
                var customerResource = MapToResourceAndUpdateIncluded(customer, querySet, included);
                customerResources.Add(customerResource);
            }

            var links = jsonApiBuilder.BuildLinks(HttpContext.Request.Path, querySet, pagedCustomers);
            var meta = jsonApiBuilder.BuildMeta(pagedCustomers);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = customerResources, Included = included });
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var jsonApiBuilder = new JsonApiBuilder();
            var querySet = jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var customer = await _customerManager.GetByIdAsync(id, querySet);

            var included = new HashSet<Resource>();
            var customerDto = MapToResourceAndUpdateIncluded(customer, querySet, included);

            var links = jsonApiBuilder.BuildLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = customerDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResourceRequest<CustomerDto> resource)
        {
            var jsonApiBuilder = new JsonApiBuilder();
            var customer = _mapper.Map<Customer>(resource.Data);

            customer = await _customerManager.Create(customer);
            var customerDto = _mapper.Map<CustomerDto>(customer);

            var links = jsonApiBuilder.BuildLinks(HttpContext.Request.Path, customerDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = customerDto });
        }

        [HttpGet("{customerId}/contacts")]
        [HttpGet("{customerId}/links/contacts")]
        public async Task<IActionResult> GetRelatedContactsByCustomerId(int customerId)
        {
            var jsonApiBuilder = new JsonApiBuilder();
            var querySet = jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedContacts = await _contactManager.GetAllByCustomerIdAsync(customerId, querySet);
            var contactResources = _mapper.Map<IEnumerable<ContactDto>>(pagedContacts.Items);

            var links = jsonApiBuilder.BuildLinks(HttpContext.Request.Path, querySet, pagedContacts);
            var meta = jsonApiBuilder.BuildMeta(pagedContacts);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = contactResources });
        }

        [HttpGet("{customerId}/buildings")]
        [HttpGet("{customerId}/links/buildings")]
        public async Task<IActionResult> GetRelatedBuildingsById(int customerId)
        {
            var jsonApiBuilder = new JsonApiBuilder();
            var querySet = jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedBuildings = await _buildingManager.GetAllByCustomerIdAsync(customerId, querySet);
            var buildingResources = _mapper.Map<IEnumerable<BuildingDto>>(pagedBuildings.Items);

            var links = jsonApiBuilder.BuildLinks(HttpContext.Request.Path, querySet, pagedBuildings);
            var meta = jsonApiBuilder.BuildMeta(pagedBuildings);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = buildingResources });
        }

        private CustomerDto MapToResourceAndUpdateIncluded(Customer customer, QuerySet querySet, ISet<Resource> included)
        {
            var resource = _mapper.Map<CustomerDto>(customer);

            if (querySet.Include.Contains("country") && customer.Country != null)
            {
                resource.Relationships["country"].Data = _mapper.Map<RelationResource>(customer.Country);
                included.Add(_mapper.Map<CountryDto>(customer.Country));
            }

            if (querySet.Include.Contains("language") && customer.Language != null)
            {
                resource.Relationships["language"].Data = _mapper.Map<RelationResource>(customer.Language);
                included.Add(_mapper.Map<LanguageDto>(customer.Language));
            }

            if (querySet.Include.Contains("postal-code") && customer.PostalCode != null)
            {
                resource.Relationships["postal-code"].Data = _mapper.Map<RelationResource>(customer.PostalCode);
                included.Add(_mapper.Map<PostalCodeDto>(customer.PostalCode));
            }

            if (querySet.Include.Contains("honorific-prefix") && customer.HonorificPrefix != null)
            {
                resource.Relationships["honorific-prefix"].Data = _mapper.Map<RelationResource>(customer.HonorificPrefix);
                included.Add(_mapper.Map<HonorificPrefixDto>(customer.HonorificPrefix));
            }

            if (querySet.Include.Contains("contacts") && customer.Contacts.Count() > 0)
            {
                resource.Relationships["contacts"].Data = _mapper.Map<IEnumerable<RelationResource>>(customer.Contacts);
                included.UnionWith(_mapper.Map<IEnumerable<ContactDto>>(customer.Contacts));
            }

            if (querySet.Include.Contains("buildings") && customer.Buildings.Count() > 0)
            {
                resource.Relationships["buildings"].Data = _mapper.Map<IEnumerable<RelationResource>>(customer.Buildings);
                included.UnionWith(_mapper.Map<IEnumerable<BuildingDto>>(customer.Buildings));
            }

            if (querySet.Include.Contains("telephones") && customer.Telephones.Count() > 0)
            {
                resource.Relationships["telephones"].Data = _mapper.Map<IEnumerable<RelationResource>>(customer.Telephones);
                included.UnionWith(_mapper.Map<IEnumerable<TelephoneDto>>(customer.Telephones));
            }

            return resource;
        }
    }
} 