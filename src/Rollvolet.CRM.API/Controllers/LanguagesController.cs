using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageManager _languageManager;
        private readonly IMapper _mapper;

        public LanguagesController(ILanguageManager languageManager, IMapper mapper)
        {
            _languageManager = languageManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var languages = await _languageManager.GetAllAsync();

            var languageDtos = _mapper.Map<IEnumerable<LanguageDto>>(languages);
            var links = new CollectionLinks() { Self = HttpContext.Request.Path };

            return Ok(new ResourceResponse() { Links = links, Data = languageDtos });
        }
    }
}