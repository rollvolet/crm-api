using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly IPaymentManager _paymentManager;
        private readonly IMapper _mapper;

        public PaymentsController(IPaymentManager paymentManager, IMapper mapper)
        {
            _paymentManager = paymentManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var payments = await _paymentManager.GetAllAsync();

            var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(payments);
            var links = new CollectionLinks() { Self = HttpContext.Request.Path };

            return Ok(new ResourceResponse() { Links = links, Data = paymentDtos });
        }
    }
}