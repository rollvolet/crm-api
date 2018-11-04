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
    [Route("way-of-entries")]
    [Authorize]
    public class WayOfEntriesController : Controller
    {
        private readonly IWayOfEntryManager _wayOfEntryManager;
        private readonly IMapper _mapper;

        public WayOfEntriesController(IWayOfEntryManager wayOfEntryManager, IMapper mapper)
        {
            _wayOfEntryManager = wayOfEntryManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var wayOfEntries = await _wayOfEntryManager.GetAllAsync();

            var wayOfEntryDtos = _mapper.Map<IEnumerable<WayOfEntryDto>>(wayOfEntries);
            var links = new CollectionLinks() { Self = HttpContext.Request.Path };

            return Ok(new ResourceResponse() { Links = links, Data = wayOfEntryDtos });
        }
    }
}