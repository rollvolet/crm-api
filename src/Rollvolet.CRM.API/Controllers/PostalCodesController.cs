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
    [Route("postal-codes")]
    [Authorize]
    public class PostalCodesontroller : ControllerBase
    {
        private readonly IPostalCodeManager _postalCodeManager;
        private readonly IMapper _mapper;

        public PostalCodesontroller(IPostalCodeManager postalCodeManager, IMapper mapper)
        {
            _postalCodeManager = postalCodeManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var postalCodes = await _postalCodeManager.GetAllAsync();

            var postalCodeDtos = _mapper.Map<IEnumerable<PostalCodeDto>>(postalCodes);
            var links = new CollectionLinks() { Self = HttpContext.Request.Path };

            return Ok(new ResourceResponse() { Links = links, Data = postalCodeDtos });
        }
    }
}