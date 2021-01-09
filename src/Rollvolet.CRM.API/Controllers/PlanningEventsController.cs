using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.APIContracts.DTO.Interventions;
using Microsoft.AspNetCore.Http;
using Rollvolet.CRM.APIContracts.DTO.PlanningEvents;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("planning-events")]
    public class PlanningEventsController : ControllerBase
    {
        private readonly IPlanningEventManager _planningEventManager;
        private readonly IInterventionManager _interventionManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public PlanningEventsController(IPlanningEventManager planningEventManager, IInterventionManager interventionManager,
                                         IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _planningEventManager = planningEventManager;
            _interventionManager = interventionManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var planningEvent = await _planningEventManager.GetByIdAsync(id, querySet);

            var planningEventDto = _mapper.Map<PlanningEventDto>(planningEvent, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(planningEvent, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = planningEventDto, Included = included });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ResourceRequest<PlanningEventRequestDto> resource)
        {
            if (resource.Data.Type != "planning-events" || resource.Data.Id != id) return StatusCode(409);

            var planningEvent = _mapper.Map<PlanningEvent>(resource.Data);

            planningEvent = await _planningEventManager.UpdateAsync(planningEvent);

            var planningEventDto = _mapper.Map<PlanningEventDto>(planningEvent);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = planningEventDto });
        }

        [HttpGet("{planningEventId}/intervention")]
        [HttpGet("{planningEventId}/links/intervention")]
        public async Task<IActionResult> GetRelatedInterventionByIdAsync(int planningEventId)
        {
            InterventionDto interventionDto;
            try
            {
                var intervention = await _interventionManager.GetByPlanningEventIdAsync(planningEventId);
                interventionDto = _mapper.Map<InterventionDto>(intervention);
            }
            catch (EntityNotFoundException)
            {
                interventionDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = interventionDto });
        }
    }
}
