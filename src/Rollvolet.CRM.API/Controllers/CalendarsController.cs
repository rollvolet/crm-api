using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.Domain.Contracts.MsGraph;
using Rollvolet.CRM.Domain.Exceptions;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class CalendarsController : Controller
    {
        private readonly IGraphApiService _graphApiService;

        public CalendarsController(IGraphApiService graphApiService)
        {
            _graphApiService = graphApiService;
        }

        [HttpGet("planning/{msObjectId}/subject")]
        public async Task<IActionResult> GetPlanningSubject(string msObjectId)
        {
            try
            {
                var subject = await _graphApiService.GetPlanningSubject(msObjectId);
                return Ok(new { Subject = subject });
            }
            catch (EntityNotFoundException)
            {
                return Ok(new { Subject = (string) null });
            }
        }
    }
}