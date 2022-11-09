using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.Domain.Managers.Interfaces;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CasesController : ControllerBase
    {
        private readonly ICaseManager _caseManager;
        private readonly IMapper _mapper;

        public CasesController(ICaseManager caseManager, IMapper mapper)
        {
            _caseManager = caseManager;
             _mapper = mapper;
        }

        [HttpPost("contact-and-building")]
        public async Task<IActionResult> UpdateContactAndBuildingAsync([FromBody] ContactAndBuildingDto contactAndBuildingDto)
        {
            await _caseManager.UpdateContactAndBuildingAsync(
                TryParseNullable(contactAndBuildingDto.ContactId),
                TryParseNullable(contactAndBuildingDto.BuildingId),
                TryParseNullable(contactAndBuildingDto.RequestId),
                TryParseNullable(contactAndBuildingDto.InterventionId),
                TryParseNullable(contactAndBuildingDto.OfferId),
                TryParseNullable(contactAndBuildingDto.OrderId),
                TryParseNullable(contactAndBuildingDto.InvoiceId));

            return NoContent();
        }

        public int? TryParseNullable(string val)
        {
            if (val == null)
            {
                return null;
            }
            else
            {
                int outValue;
                return int.TryParse(val, out outValue) ? (int?) outValue : null;
            }
        }
    }
}