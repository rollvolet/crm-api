using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.APIContracts.DTO.ErrorNotifications;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("error-notifications")]
    [Authorize]
    public class ErrorNotificationsController : ControllerBase
    {
        private readonly IErrorNotificationManager _errorNotificationManager;
        private readonly IMapper _mapper;

        public ErrorNotificationsController(IErrorNotificationManager errorNotificationManager, IMapper mapper)
        {
            _errorNotificationManager = errorNotificationManager;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<ErrorNotificationRequestDto> resource)
        {
            if (resource.Data.Type != "error-notifications") return StatusCode(409);

            var errorNotification = _mapper.Map<ErrorNotification>(resource.Data);

            await _errorNotificationManager.CreateAsync(errorNotification);

            return NoContent();
        }
    }
}