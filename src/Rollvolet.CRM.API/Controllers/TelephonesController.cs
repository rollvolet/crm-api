using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.APIContracts.DTO.Telephones;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class TelephonesController : Controller
    {
        private readonly ITelephoneManager _telephoneManager;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public TelephonesController(ITelephoneManager telephoneManager, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _telephoneManager = telephoneManager;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResourceRequest<TelephoneRequestDto> resource)
        {
            var telephone = _mapper.Map<Telephone>(resource.Data);

            telephone = await _telephoneManager.CreateAsync(telephone);
            var telephoneDto = _mapper.Map<TelephoneDto>(telephone);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, telephoneDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = telephoneDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _telephoneManager.DeleteAsync(id);

            return NoContent();
        }
    }
}