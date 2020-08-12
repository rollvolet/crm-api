using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.Domain.Contracts.MsGraph;
using Rollvolet.CRM.Domain.Exceptions;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CalendarsController : ControllerBase
    {
        private readonly IGraphApiCalendarService _calendarService;

        public CalendarsController(IGraphApiCalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        [HttpGet("planning/{msObjectId}/subject")]
        public async Task<IActionResult> GetPlanningSubjectAsync(string msObjectId)
        {
            try
            {
                var subject = await _calendarService.GetPlanningSubjectAsync(msObjectId);
                return Ok(new { Subject = subject });
            }
            catch (EntityNotFoundException)
            {
                return Ok(new { Subject = (string) null });
            }
        }
    }
}