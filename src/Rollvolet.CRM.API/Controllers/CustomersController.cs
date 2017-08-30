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
        private readonly IMapper _mapper;

        public CustomersController(ICustomerManager customerManager, IMapper mapper)
        {
            _customerManager = customerManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var jsonApiBuilder = new JsonApiBuilder();
            var querySet = jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedCustomers = await _customerManager.GetAllAsync(querySet);
            var mappedCustomers = _mapper.Map<IEnumerable<CustomerDto>>(pagedCustomers.Items);

            var links = jsonApiBuilder.BuildLinks(HttpContext.Request.Path, querySet, pagedCustomers);
            var meta = jsonApiBuilder.BuildMeta(pagedCustomers);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = mappedCustomers });
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var jsonApiBuilder = new JsonApiBuilder();

            var customer = await _customerManager.GetByIdAsync(id);
            var mappedCustomer = _mapper.Map<CustomerDto>(customer);

            var links = jsonApiBuilder.BuildLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = customer});
        }

        [HttpGet("{id}/relationships/contacts")]
        public async Task<IActionResult> GetRelatedContactsById(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}/relationships/buildings")]
        public async Task<IActionResult> GetRelatedBuildingsById(int id)
        {
            throw new NotImplementedException();
        }
    }
} 