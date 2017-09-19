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
    public class CustomersController : JsonApiController
    {
        private readonly ICustomerManager _customerManager;
        private readonly IContactManager _contactManager;
        private readonly IBuildingManager _buildingManager;

        public CustomersController(ICustomerManager customerManager, IContactManager contactManager, IBuildingManager buildingManager, IMapper mapper) : base(mapper)
        {
            _customerManager = customerManager;
            _contactManager = contactManager;
            _buildingManager = buildingManager;
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

            MapOneAndUpdateIncluded<Country, CountryDto>("country", customer.Country, querySet, resource, included);
            MapOneAndUpdateIncluded<Language, LanguageDto>("language", customer.Language, querySet, resource, included);
            MapOneAndUpdateIncluded<PostalCode, PostalCodeDto>("postal-code", customer.PostalCode, querySet, resource, included);
            MapOneAndUpdateIncluded<HonorificPrefix, HonorificPrefixDto>("honorific-preix", customer.HonorificPrefix, querySet, resource, included);

            MapManyAndUpdateIncluded<Contact, ContactDto>("contacts", customer.Contacts, querySet, resource, included);
            MapManyAndUpdateIncluded<Building, BuildingDto>("buildings", customer.Buildings, querySet, resource, included);
            MapManyAndUpdateIncluded<Telephone, TelephoneDto>("telephones", customer.Telephones, querySet, resource, included);

            return resource;
        }

    }
} 