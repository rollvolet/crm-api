using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.API.Builders;
using Rollvolet.CRM.API.Builders.Interfaces;
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
        private readonly ITelephoneManager _telephoneManager;

        public CustomersController(ICustomerManager customerManager, IContactManager contactManager, IBuildingManager buildingManager, 
                                    ITelephoneManager telephoneManager, IMapper mapper, IJsonApiBuilder jsonApiBuilder) : base(mapper, jsonApiBuilder)
        {
            _customerManager = customerManager;
            _contactManager = contactManager;
            _buildingManager = buildingManager;
            _telephoneManager = telephoneManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedCustomers = await _customerManager.GetAllAsync(querySet);

            var included = new HashSet<Resource>();

            var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(pagedCustomers.Items);
            
            // var customerResources = new List<CustomerDto>();
            // foreach (var customer in pagedCustomers.Items)
            // {
            //     var customerResource = MapToResourceAndUpdateIncluded(customer, querySet, included);
            //     customerResources.Add(customerResource);
            // }

            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedCustomers);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedCustomers);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = customerDtos, Included = included });
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var customer = await _customerManager.GetByIdAsync(id, querySet);

            var included = new HashSet<Resource>();
            // var customerDto = MapToResourceAndUpdateIncluded(customer, querySet, included);
            var customerDto = _mapper.Map<CustomerDto>(customer);

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = customerDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResourceRequest<CustomerDto> resource)
        {
            var customer = _mapper.Map<Customer>(resource.Data);

            customer = await _customerManager.Create(customer);
            var customerDto = _mapper.Map<CustomerDto>(customer);

            var links = _jsonApiBuilder.BuildLinks(HttpContext.Request.Path, customerDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = customerDto });
        }

        [HttpGet("{customerId}/contacts")]
        [HttpGet("{customerId}/links/contacts")]
        public async Task<IActionResult> GetRelatedContactsByCustomerId(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedContacts = await _contactManager.GetAllByCustomerIdAsync(customerId, querySet);
            var contactResources = _mapper.Map<IEnumerable<ContactDto>>(pagedContacts.Items);

            var links = _jsonApiBuilder.BuildLinks(HttpContext.Request.Path, querySet, pagedContacts);
            var meta = _jsonApiBuilder.BuildMeta(pagedContacts);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = contactResources });
        }

        [HttpGet("{customerId}/buildings")]
        [HttpGet("{customerId}/links/buildings")]
        public async Task<IActionResult> GetRelatedBuildingsById(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedBuildings = await _buildingManager.GetAllByCustomerIdAsync(customerId, querySet);
            var buildingResources = _mapper.Map<IEnumerable<BuildingDto>>(pagedBuildings.Items);

            var links = _jsonApiBuilder.BuildLinks(HttpContext.Request.Path, querySet, pagedBuildings);
            var meta = _jsonApiBuilder.BuildMeta(pagedBuildings);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = buildingResources });
        }

        [HttpGet("{customerId}/telephones")]
        [HttpGet("{customerId}/links/telephones")]
        public async Task<IActionResult> GetRelatedTelephonesById(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedTelephones = await _telephoneManager.GetAllByCustomerIdAsync(customerId, querySet);

            var included = new HashSet<Resource>();
            var telephoneResources = new List<TelephoneDto>();

            foreach (var telephone in pagedTelephones.Items)
            {
                var telephoneResource = MapToResourceAndUpdateIncluded(telephone, querySet, included);
                telephoneResources.Add(telephoneResource);
            }

            var links = _jsonApiBuilder.BuildLinks(HttpContext.Request.Path, querySet, pagedTelephones);
            var meta = _jsonApiBuilder.BuildMeta(pagedTelephones);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = telephoneResources, Included = included });
        }

        // TODO: refactor to extension method of CustomerDto
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

        // TODO: refactor to extension method of TelephoneDto
        private TelephoneDto MapToResourceAndUpdateIncluded(Telephone telephone, QuerySet querySet, ISet<Resource> included)
        {
            var resource = _mapper.Map<TelephoneDto>(telephone);

            MapOneAndUpdateIncluded<Country, CountryDto>("country", telephone.Country, querySet, resource, included);
            MapOneAndUpdateIncluded<TelephoneType, TelephoneTypeDto>("telephone-type", telephone.TelephoneType, querySet, resource, included);

            return resource;
        }

    }
} 