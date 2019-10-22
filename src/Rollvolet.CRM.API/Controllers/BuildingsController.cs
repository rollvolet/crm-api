using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.DTO.Buildings;
using Rollvolet.CRM.APIContracts.DTO.Invoices;
using Rollvolet.CRM.APIContracts.DTO.Requests;
using Rollvolet.CRM.APIContracts.DTO.Telephones;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class BuildingsController : ControllerBase
    {
        private readonly IBuildingManager _buildingManager;
        private readonly ITelephoneManager _telephoneManager;
        private readonly ICountryManager _countryManager;
        private readonly ILanguageManager _languageManager;
        private readonly IHonorificPrefixManager _honorificPrefixManager;
        private readonly IRequestManager _requestManager;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public BuildingsController(IBuildingManager buildingManager, ITelephoneManager telephoneManager, ICountryManager countryManager,
                                    ILanguageManager languageManager, IHonorificPrefixManager honorificPrefixManager,
                                    IRequestManager requestManager, IInvoiceManager invoiceManager,
                                    IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _buildingManager = buildingManager;
            _telephoneManager = telephoneManager;
            _countryManager = countryManager;
            _languageManager = languageManager;
            _honorificPrefixManager = honorificPrefixManager;
            _requestManager = requestManager;
            _invoiceManager = invoiceManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var building = await _buildingManager.GetByIdAsync(id, querySet);

            var buildingDto = _mapper.Map<BuildingDto>(building, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(building, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = buildingDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<BuildingRequestDto> resource)
        {
            if (resource.Data.Type != "buildings") return StatusCode(409);

            var building = _mapper.Map<Building>(resource.Data);

            building = await _buildingManager.CreateAsync(building);

            var buildingDto = _mapper.Map<BuildingDto>(building);
            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, buildingDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = buildingDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ResourceRequest<BuildingRequestDto> resource)
        {
            if (resource.Data.Type != "buildings" || resource.Data.Id != id) return StatusCode(409);

            var building = _mapper.Map<Building>(resource.Data);

            building = await _buildingManager.UpdateAsync(building);

            var buildingDto = _mapper.Map<BuildingDto>(building);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = buildingDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _buildingManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("{buildingId}/requests")]
        [HttpGet("{buildingId}/links/requests")]
        public async Task<IActionResult> GetRelatedRequestsByIdAsync(int buildingId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedRequests = await _requestManager.GetAllByBuildingIdAsync(buildingId, querySet);

            var requestDtos = _mapper.Map<IEnumerable<RequestDto>>(pagedRequests.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedRequests.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedRequests);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedRequests);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = requestDtos, Included = included });
        }

        [HttpGet("{buildingId}/invoices")]
        [HttpGet("{buildingId}/links/invoices")]
        public async Task<IActionResult> GetRelatedInvoicesByIdAsync(int buildingId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedInvoices = await _invoiceManager.GetAllByBuildingIdAsync(buildingId, querySet);

            var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(pagedInvoices.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedInvoices.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedInvoices);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedInvoices);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = invoiceDtos, Included = included });
        }

        [HttpGet("{buildingId}/telephones")]
        [HttpGet("{buildingId}/links/telephones")]
        public async Task<IActionResult> GetRelatedTelephonesByIdAsync(int buildingId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);
            querySet.Include.Fields = new string[] {"country", "telephone-type"};

            var pagedTelephones = await _telephoneManager.GetAllByBuildingIdAsync(buildingId, querySet);

            var telephoneDtos = _mapper.Map<IEnumerable<TelephoneDto>>(pagedTelephones.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedTelephones.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedTelephones);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedTelephones);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = telephoneDtos, Included = included });
        }

        [HttpGet("{buildingId}/country")]
        [HttpGet("{buildingId}/links/country")]
        public async Task<IActionResult> GetRelatedCountryByIdAsync(int buildingId)
        {
            CountryDto countryDto;
            try
            {
                var country = await _countryManager.GetByBuildingIdAsync(buildingId);
                countryDto = _mapper.Map<CountryDto>(country);
            }
            catch (EntityNotFoundException)
            {
                countryDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = countryDto });
        }

        [HttpGet("{buildingId}/language")]
        [HttpGet("{buildingId}/links/language")]
        public async Task<IActionResult> GetRelatedLanguageByIdAsync(int buildingId)
        {
            LanguageDto languageDto;
            try
            {
                var language = await _languageManager.GetByBuildingIdAsync(buildingId);
                languageDto = _mapper.Map<LanguageDto>(language);
            }
            catch (EntityNotFoundException)
            {
                languageDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = languageDto });
        }

        [HttpGet("{buildingId}/honorific-prefix")]
        [HttpGet("{buildingId}/links/honorific-prefix")]
        public async Task<IActionResult> GetRelatedHonorificPrefixByIdAsync(int buildingId)
        {
            HonorificPrefixDto honorificPrefixDto;
            try
            {
                var honorificPrefix = await _honorificPrefixManager.GetByBuildingIdAsync(buildingId);
                honorificPrefixDto = _mapper.Map<HonorificPrefixDto>(honorificPrefix);
            }
            catch (EntityNotFoundException)
            {
                honorificPrefixDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = honorificPrefixDto });
        }
    }
}