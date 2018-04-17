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
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("postal-codes")]
    [Authorize]
    public class PostalCodesontroller : Controller
    {
        private readonly IPostalCodeManager _postalCodeManager;
        private readonly IMapper _mapper;

        public PostalCodesontroller(IPostalCodeManager postalCodeManager, IMapper mapper)
        {
            _postalCodeManager = postalCodeManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var postalCodes = await _postalCodeManager.GetAllAsync();

            var postalCodeDtos = _mapper.Map<IEnumerable<PostalCodeDto>>(postalCodes);
            var links = new CollectionLinks() { Self = HttpContext.Request.Path };

            return Ok(new ResourceResponse() { Links = links, Data = postalCodeDtos });
        }
    }
}