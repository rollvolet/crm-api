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
using Rollvolet.CRM.APIContracts.DTO.Visits;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class VisitsController : Controller
    {
        private readonly IVisitManager _visitManager;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public VisitsController(IVisitManager visitManager,
                                    IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _visitManager = visitManager;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<VisitRequestDto> resource)
        {
            if (resource.Data.Type != "visits") return StatusCode(409);

            var visit = _mapper.Map<Visit>(resource.Data);

            visit = await _visitManager.CreateAsync(visit);
            var visitDto = _mapper.Map<VisitDto>(visit);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, visitDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = visitDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ResourceRequest<VisitRequestDto> resource)
        {
            if (resource.Data.Type != "visits" || resource.Data.Id != id) return StatusCode(409);

            var visit = _mapper.Map<Visit>(resource.Data);

            visit = await _visitManager.UpdateAsync(visit);

            var visitDto = _mapper.Map<VisitDto>(visit);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = visitDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _visitManager.DeleteAsync(id);

            return NoContent();
        }
    }
}