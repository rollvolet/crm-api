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
using Rollvolet.CRM.APIContracts.DTO.CalendarEvents;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("calendar-events")]
    [Authorize]
    public class CalendarEventsController : Controller
    {
        private readonly ICalendarEventManager _calendarEventManager;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public CalendarEventsController(ICalendarEventManager calendarEventManager,
                                    IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _calendarEventManager = calendarEventManager;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<CalendarEventRequestDto> resource)
        {
            if (resource.Data.Type != "calendar-events") return StatusCode(409);

            var calendarEvent = _mapper.Map<CalendarEvent>(resource.Data);

            calendarEvent = await _calendarEventManager.CreateAsync(calendarEvent);
            var calendarEventDto = _mapper.Map<CalendarEventDto>(calendarEvent);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, calendarEventDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = calendarEventDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ResourceRequest<CalendarEventRequestDto> resource)
        {
            if (resource.Data.Type != "calendar-events" || resource.Data.Id != id) return StatusCode(409);

            var calendarEvent = _mapper.Map<CalendarEvent>(resource.Data);

            calendarEvent = await _calendarEventManager.UpdateAsync(calendarEvent);

            var calendarEventDto = _mapper.Map<CalendarEventDto>(calendarEvent);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = calendarEventDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _calendarEventManager.DeleteAsync(id);

            return NoContent();
        }
    }
}