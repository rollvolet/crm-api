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
    [Route("vat-rates")]
    //[Authorize]
    public class VatRatesController : Controller
    {
        private readonly IVatRateManager _vatRateManager;
        private readonly IMapper _mapper;

        public VatRatesController(IVatRateManager vatRateManager, IMapper mapper)
        {
            _vatRateManager = vatRateManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vatRates = await _vatRateManager.GetAllAsync();

            var vatRateDtos = _mapper.Map<IEnumerable<VatRateDto>>(vatRates);
            var links = new CollectionLinks() { Self = HttpContext.Request.Path };

            return Ok(new ResourceResponse() { Links = links, Data = vatRateDtos });
        }
    }
}