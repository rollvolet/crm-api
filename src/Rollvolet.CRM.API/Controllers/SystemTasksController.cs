using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using System.Threading.Tasks;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("system-tasks")]
    [Authorize]
    public class SystemTasksController : ControllerBase
    {
        private readonly ISystemTaskManager _systemTaskManager;

        public SystemTasksController(ISystemTaskManager systemTaskManager)
        {
            _systemTaskManager = systemTaskManager;
        }

        [HttpPost("recalculate-search-names")]
        public async Task<IActionResult> RecalculateCustomersSearchName()
        {
            await _systemTaskManager.RecalculateSearchNames();
            return Accepted();
        }
    }
}
