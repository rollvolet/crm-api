using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("honorific-prefixes")]
    [Authorize]
    public class HonorificPrefixesController : ControllerBase
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