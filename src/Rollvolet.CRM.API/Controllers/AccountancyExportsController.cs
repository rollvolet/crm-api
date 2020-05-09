using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.APIContracts.DTO.AccountancyExports;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("accountancy-exports")]
    [Authorize]
    public class AccountancyExportsController : ControllerBase
    {
        private readonly IAccountancyExportManager _accountancyExportManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public AccountancyExportsController(IAccountancyExportManager accountancyExportManager,
                                            IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _accountancyExportManager = accountancyExportManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedAccountancyExports = await _accountancyExportManager.GetAllAsync(querySet);

            var accountancyExportDtos = _mapper.Map<IEnumerable<AccountancyExportDto>>(pagedAccountancyExports.Items);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedAccountancyExports);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedAccountancyExports);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = accountancyExportDtos });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var accountancyExport = await _accountancyExportManager.GetByIdAsync(id, querySet);

            var accountancyExportDto = _mapper.Map<AccountancyExportDto>(accountancyExport);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = accountancyExportDto });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<AccountancyExportRequestDto> resource)
        {
            if (resource.Data.Type != "accountancy-exports") return StatusCode(409);

            var accountancyExport = _mapper.Map<AccountancyExport>(resource.Data);

            accountancyExport = await _accountancyExportManager.CreateAsync(accountancyExport);
            var accountancyExportDto = _mapper.Map<AccountancyExportDto>(accountancyExport);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, accountancyExportDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = accountancyExportDto });
        }
    }
}
