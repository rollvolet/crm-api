using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.API.Builders;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.DTO.Deposits;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class DepositsController : Controller
    {
        private readonly IDepositManager _depositManager;
        private readonly IPaymentManager _paymentManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;
        private readonly ILogger _logger;

        public DepositsController(IDepositManager depositManager, IPaymentManager paymentManager, IIncludedCollector includedCollector,
                                    IMapper mapper, IJsonApiBuilder jsonApiBuilder, ILogger<OffersController> logger)
        {
            _depositManager = depositManager;
            _paymentManager = paymentManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResourceRequest<DepositRequestDto> resource)
        {
            if (resource.Data.Type != "deposits") return StatusCode(409);

            var deposit = _mapper.Map<Deposit>(resource.Data);

            deposit = await _depositManager.CreateAsync(deposit);
            var depositDto = _mapper.Map<DepositDto>(deposit);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, depositDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = depositDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ResourceRequest<DepositRequestDto> resource)
        {
            if (resource.Data.Type != "deposits" || resource.Data.Id != id) return StatusCode(409);

            var deposit = _mapper.Map<Deposit>(resource.Data);

            deposit = await _depositManager.UpdateAsync(deposit);

            var depositDto = _mapper.Map<DepositDto>(deposit);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = depositDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _depositManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("{depositId}/payment")]
        [HttpGet("{depositId}/links/payment")]
        public async Task<IActionResult> GetRelatedPaymentById(int depositId)
        {
            PaymentDto paymentDto;
            try
            {
                var payment = await _paymentManager.GetByDepositIdAsync(depositId);
                paymentDto = _mapper.Map<PaymentDto>(payment);
            }
            catch (EntityNotFoundException)
            {
                paymentDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = paymentDto });
        }
    }
}