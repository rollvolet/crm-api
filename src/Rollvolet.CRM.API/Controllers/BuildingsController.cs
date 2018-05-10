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
using Rollvolet.CRM.APIContracts.DTO.Buildings;
using Rollvolet.CRM.APIContracts.DTO.Telephones;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class BuildingsController : Controller
    {
        private readonly IBuildingManager _buildingManager;
        private readonly ITelephoneManager _telephoneManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public BuildingsController(IBuildingManager buildingManager, ITelephoneManager telephoneManager, IIncludedCollector includedCollector,
                                    IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _buildingManager = buildingManager;
            _telephoneManager = telephoneManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var building = await _buildingManager.GetByIdAsync(id, querySet);

            var buildingDto = _mapper.Map<BuildingDto>(building, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(building, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = buildingDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResourceRequest<BuildingRequestDto> resource)
        {
            if (resource.Data.Type != "buildings") return StatusCode(409);

            var building = _mapper.Map<Building>(resource.Data);

            building = await _buildingManager.CreateAsync(building);

            var buildingDto = _mapper.Map<BuildingDto>(building);
            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, buildingDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = buildingDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ResourceRequest<BuildingRequestDto> resource)
        {
            if (resource.Data.Type != "buildings" || resource.Data.Id != id) return StatusCode(409);

            var building = _mapper.Map<Building>(resource.Data);

            building = await _buildingManager.UpdateAsync(building);

            var buildingDto = _mapper.Map<BuildingDto>(building);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = buildingDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _buildingManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("{buildingId}/telephones")]
        [HttpGet("{buildingId}/links/telephones")]
        public async Task<IActionResult> GetRelatedTelephonesById(int buildingId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);
            querySet.Include.Fields = new string[] {"country", "telephone-type"};

            var pagedTelephones = await _telephoneManager.GetAllByBuildingIdAsync(buildingId, querySet);

            var telephoneDtos = _mapper.Map<IEnumerable<TelephoneDto>>(pagedTelephones.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedTelephones.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedTelephones);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedTelephones);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = telephoneDtos, Included = included });
        }

    }
}