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
    [Route("telephone-types")]
    [Authorize]
    public class TelephoneTypesController : ControllerBase
    {
        private readonly ITelephoneTypeManager _telephoneTypeManager;
        private readonly IMapper _mapper;

        public TelephoneTypesController(ITelephoneTypeManager telephoneTypeManager, IMapper mapper)
        {
            _telephoneTypeManager = telephoneTypeManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var telephoneTypes = await _telephoneTypeManager.GetAllAsync();

            var telephoneTypeDtos = _mapper.Map<IEnumerable<TelephoneTypeDto>>(telephoneTypes);
            var links = new CollectionLinks() { Self = HttpContext.Request.Path };

            return Ok(new ResourceResponse() { Links = links, Data = telephoneTypeDtos });
        }
    }
}