using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("api/[controller]")]
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
            var customers = await _customerManager.GetAllAsync();
            var mappedCustomers = _mapper.Map<IEnumerable<CustomerDto>>(customers);
            return Ok(new ResourceResponse() { Data = mappedCustomers });
        }
        
    }
} 