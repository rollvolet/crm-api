using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.API.Builders;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;

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
            var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(pagedCustomers.Items);

            var links = jsonApiBuilder.BuildLinks(HttpContext.Request.Path, querySet, pagedCustomers);
            var meta = jsonApiBuilder.BuildMeta(pagedCustomers);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = customerDtos });
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var jsonApiBuilder = new JsonApiBuilder();

            var customer = await _customerManager.GetByIdAsync(id);
            var customerDto = _mapper.Map<CustomerDto>(customer);

            var links = jsonApiBuilder.BuildLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = customer});
        }

        [HttpGet("{customerId}/relationships/contacts")]
        public async Task<IActionResult> GetRelatedContactsByCustomerId(int customerId)
        {
            var jsonApiBuilder = new JsonApiBuilder();
            var querySet = jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedContacts = await _contactManager.GetAllByCustomerIdAsync(customerId, querySet);
            var contactDtos = _mapper.Map<IEnumerable<ContactDto>>(pagedContacts.Items);

            var links = jsonApiBuilder.BuildLinks(HttpContext.Request.Path, querySet, pagedContacts);
            var meta = jsonApiBuilder.BuildMeta(pagedContacts);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = contactDtos });
        }

        [HttpGet("{customerId}/relationships/buildings")]
        public async Task<IActionResult> GetRelatedBuildingsById(int customerId)
        {
            var jsonApiBuilder = new JsonApiBuilder();
            var querySet = jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedBuildings = await _buildingManager.GetAllByCustomerIdAsync(customerId, querySet);
            var buildingDtos = _mapper.Map<IEnumerable<BuildingDto>>(pagedBuildings.Items);

            var links = jsonApiBuilder.BuildLinks(HttpContext.Request.Path, querySet, pagedBuildings);
            var meta = jsonApiBuilder.BuildMeta(pagedBuildings);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = buildingDtos });
        }
    }
} 