using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.Domain.Managers.Interfaces;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class CasesController : Controller
    {
        private readonly ICaseManager _caseManager;
        private readonly IMapper _mapper;

        public CasesController(ICaseManager caseManager, IMapper mapper)
        {
            _caseManager = caseManager;
             _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCase([FromQuery] int? requestId, [FromQuery] int? offerId, [FromQuery] int? orderId, [FromQuery] int? invoiceId)
        {
            // TOOD return 400 if multiple query params set

            var caseObject = await _caseManager.GetCase(requestId, offerId, orderId, invoiceId);
            var mappedCase = _mapper.Map<CaseDto>(caseObject);

            return Ok(mappedCase);
        }        
    }
}