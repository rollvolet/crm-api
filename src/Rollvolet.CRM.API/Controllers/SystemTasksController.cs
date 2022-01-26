using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using System.Threading.Tasks;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("system-tasks")]
    public class SystemTasksController : ControllerBase
    {
        private readonly ISystemTaskManager _systemTaskManager;

        public SystemTasksController(ISystemTaskManager systemTaskManager)
        {
            _systemTaskManager = systemTaskManager;
        }

        [HttpPost("rename-offer-documents")]
        public async Task<IActionResult> RenameOfferDocuments()
        {
            await _systemTaskManager.RenameOfferDocuments();
            return Accepted();
        }

        [HttpPost("recalculate-search-names")]
        public async Task<IActionResult> RecalculateCustomersSearchName()
        {
            await _systemTaskManager.RecalculateSearchNames();
            return Accepted();
        }
    }
}
