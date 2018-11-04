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
    [Route("honorific-prefixes")]
    [Authorize]
    public class HonorificPrefixesController : Controller
    {
        private readonly IHonorificPrefixManager _honorificPrefixManager;
        private readonly IMapper _mapper;

        public HonorificPrefixesController(IHonorificPrefixManager honorificPrefixManager, IMapper mapper)
        {
            _honorificPrefixManager = honorificPrefixManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var honorificPrefixes = await _honorificPrefixManager.GetAllAsync();

            var honorificPrefixDtos = _mapper.Map<IEnumerable<HonorificPrefixDto>>(honorificPrefixes);
            var links = new CollectionLinks() { Self = HttpContext.Request.Path };

            return Ok(new ResourceResponse() { Links = links, Data = honorificPrefixDtos });
        }
    }
}