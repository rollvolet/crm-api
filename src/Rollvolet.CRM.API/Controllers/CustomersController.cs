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
using Rollvolet.CRM.API.Collectors;
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
        private readonly ITelephoneManager _telephoneManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public CustomersController(ICustomerManager customerManager, IContactManager contactManager, IBuildingManager buildingManager, 
                                    ITelephoneManager telephoneManager, IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _customerManager = customerManager;
            _contactManager = contactManager;
            _buildingManager = buildingManager;
            _telephoneManager = telephoneManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedCustomers = await _customerManager.GetAllAsync(querySet);

            var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(pagedCustomers.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedCustomers.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedCustomers);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedCustomers);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = customerDtos, Included = included });
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var customer = await _customerManager.GetByIdAsync(id, querySet);

            var customerDto = _mapper.Map<CustomerDto>(customer, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(customer, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = customerDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResourceRequest<CustomerDto> resource)
        {
            var customer = _mapper.Map<Customer>(resource.Data);

            customer = await _customerManager.Create(customer);
            var customerDto = _mapper.Map<CustomerDto>(customer);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, customerDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = customerDto });
        }

        [HttpGet("{customerId}/contacts")]
        [HttpGet("{customerId}/links/contacts")]
        public async Task<IActionResult> GetRelatedContactsByCustomerId(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);
            querySet.Include.Fields = new string[] {"postal-code", "country", "language"};

            var pagedContacts = await _contactManager.GetAllByCustomerIdAsync(customerId, querySet);

            var contactDtos = _mapper.Map<IEnumerable<ContactDto>>(pagedContacts.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedContacts.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedContacts);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedContacts);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = contactDtos, Included = included });
        }

        [HttpGet("{customerId}/buildings")]
        [HttpGet("{customerId}/links/buildings")]
        public async Task<IActionResult> GetRelatedBuildingsById(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedBuildings = await _buildingManager.GetAllByCustomerIdAsync(customerId, querySet);

            var buildingDtos = _mapper.Map<IEnumerable<BuildingDto>>(pagedBuildings.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedBuildings.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedBuildings);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedBuildings);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = buildingDtos, Included = included });
        }

        [HttpGet("{customerId}/telephones")]
        [HttpGet("{customerId}/links/telephones")]
        public async Task<IActionResult> GetRelatedTelephonesById(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedTelephones = await _telephoneManager.GetAllByCustomerIdAsync(customerId, querySet);

            var telephoneDtos = _mapper.Map<IEnumerable<TelephoneDto>>(pagedTelephones.Items);
            var included = _includedCollector.CollectIncluded(pagedTelephones.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedTelephones);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedTelephones);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = telephoneDtos, Included = included });
        }

    }
} 