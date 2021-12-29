using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.Business.Managers.Interfaces;
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
        private readonly ISystemTaskExecutor _systemTaskExecutor;

        public SystemTasksController(ISystemTaskManager systemTaskManager, ISystemTaskExecutor systemTaskExecutor)
        {
            _systemTaskManager = systemTaskManager;
            _systemTaskExecutor = systemTaskExecutor;
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

        [HttpPost("restore-vat-certificates")]
        public async Task<IActionResult> RestoreVatCertificates()
        {
            await _systemTaskExecutor.RestoreVatCertificates();
            return Accepted();
        }
    }
}
