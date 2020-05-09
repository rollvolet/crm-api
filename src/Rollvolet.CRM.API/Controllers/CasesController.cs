using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.Domain.Managers.Interfaces;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CasesController : ControllerBase
    {
        private readonly ICaseManager _caseManager;
        private readonly IMapper _mapper;

        public CasesController(ICaseManager caseManager, IMapper mapper)
        {
            _caseManager = caseManager;
             _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCaseAsync([FromQuery] int? requestId, [FromQuery] int? interventionId, [FromQuery] int? offerId, [FromQuery] int? orderId, [FromQuery] int? invoiceId)
        {
            var caseObject = await _caseManager.GetCaseAsync(requestId, interventionId, offerId, orderId, invoiceId);
            var mappedCase = _mapper.Map<CaseDto>(caseObject);

            return Ok(mappedCase);
        }

        [HttpPost("contact-and-building")]
        public async Task<IActionResult> UpdateContactAndBuildingAsync([FromBody] ContactAndBuildingDto contactAndBuildingDto)
        {
            await _caseManager.UpdateContactAndBuildingAsync(contactAndBuildingDto.ContactId, contactAndBuildingDto.BuildingId,
                                                        contactAndBuildingDto.RequestId, contactAndBuildingDto.InterventionId,
                                                         contactAndBuildingDto.OfferId, contactAndBuildingDto.OrderId,
                                                         contactAndBuildingDto.InvoiceId);
            return NoContent();
        }
    }
}