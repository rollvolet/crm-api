using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.API.Builders;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.DTO.Buildings;
using Rollvolet.CRM.APIContracts.DTO.Contacts;
using Rollvolet.CRM.APIContracts.DTO.Customers;
using Rollvolet.CRM.APIContracts.DTO.Offers;
using Rollvolet.CRM.APIContracts.DTO.Requests;
using Rollvolet.CRM.APIContracts.DTO.Visits;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class RequestsController : Controller
    {
        private readonly IRequestManager _requestManager;
        private readonly IWayOfEntryManager _wayOfEntryManager;
        private readonly ICustomerManager _customerManager;
        private readonly IContactManager _contactManager;
        private readonly IBuildingManager _buildingManager;
        private readonly IOfferManager _offerManager;
        private readonly IVisitManager _visitManager;
        private readonly IDocumentGenerationManager _documentGenerationManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public RequestsController(IRequestManager requestManager, IOfferManager offerManager, IWayOfEntryManager wayOfEntryManager,
                                    ICustomerManager customerManager, IContactManager contactManager, IBuildingManager buildingManager,
                                     IVisitManager visitManager, IDocumentGenerationManager documentGenerationManager,
                                     IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _requestManager = requestManager;
            _offerManager = offerManager;
            _wayOfEntryManager = wayOfEntryManager;
            _customerManager = customerManager;
            _contactManager = contactManager;
            _buildingManager = buildingManager;
            _visitManager = visitManager;
            _documentGenerationManager = documentGenerationManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedRequests = await _requestManager.GetAllAsync(querySet);

            var requestDtos = _mapper.Map<IEnumerable<RequestDto>>(pagedRequests.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedRequests.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedRequests);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedRequests);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = requestDtos, Included = included });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var request = await _requestManager.GetByIdAsync(id, querySet);

            var requestDto = _mapper.Map<RequestDto>(request, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(request, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = requestDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<RequestRequestDto> resource)
        {
            if (resource.Data.Type != "requests") return StatusCode(409);

            var request = _mapper.Map<Request>(resource.Data);

            request = await _requestManager.CreateAsync(request);
            var requestDto = _mapper.Map<RequestDto>(request);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, requestDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = requestDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ResourceRequest<RequestRequestDto> resource)
        {
            if (resource.Data.Type != "requests" || resource.Data.Id != id) return StatusCode(409);

            var request = _mapper.Map<Request>(resource.Data);

            request = await _requestManager.UpdateAsync(request);

            var requestDto = _mapper.Map<RequestDto>(request);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = requestDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _requestManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpPost("{id}/reports")]
        public async Task<IActionResult> CreateVisitReportAsync(int id)
        {
            var stream = await _documentGenerationManager.CreateVisitReportAsync(id);

            var file = new FileStreamResult(stream, "application/pdf");
            file.FileDownloadName = $"AD{id}_bezoekrapport.pdf";
            return file;
        }

        [HttpGet("{requestId}/customer")]
        [HttpGet("{requestId}/links/customer")]
        public async Task<IActionResult> GetRelatedCustomerByIdAsync(int requestId)
        {
            CustomerDto customerDto;
            try
            {
                var customer = await _customerManager.GetByRequestIdAsync(requestId);
                customerDto = _mapper.Map<CustomerDto>(customer);
            }
            catch (EntityNotFoundException)
            {
                customerDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = customerDto });
        }

        [HttpGet("{requestId}/contact")]
        [HttpGet("{requestId}/links/contact")]
        public async Task<IActionResult> GetRelatedContactByIdAsync(int requestId)
        {
            ContactDto contactDto;
            try
            {
                var contact = await _contactManager.GetByRequestIdAsync(requestId);
                contactDto = _mapper.Map<ContactDto>(contact);
            }
            catch (EntityNotFoundException)
            {
                contactDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = contactDto });
        }

        [HttpGet("{requestId}/building")]
        [HttpGet("{requestId}/links/building")]
        public async Task<IActionResult> GetRelatedBuildingByIdAsync(int requestId)
        {
            BuildingDto buildingDto;
            try
            {
                var building = await _buildingManager.GetByRequestIdAsync(requestId);
                buildingDto = _mapper.Map<BuildingDto>(building);
            }
            catch (EntityNotFoundException)
            {
                buildingDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = buildingDto });
        }

        [HttpGet("{requestId}/offer")]
        [HttpGet("{requestId}/links/offer")]
        public async Task<IActionResult> GetRelatedOfferByIdAsync(int requestId)
        {
            OfferDto offerDto;
            try
            {
                var offer = await _offerManager.GetByRequestIdAsync(requestId);
                offerDto = _mapper.Map<OfferDto>(offer);
            }
            catch (EntityNotFoundException)
            {
                offerDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = offerDto });
        }

        [HttpGet("{requestId}/way-of-entry")]
        [HttpGet("{requestId}/links/way-of-entry")]
        public async Task<IActionResult> GetRelatedWayOfEntryByIdAsync(int requestId)
        {
            WayOfEntryDto wayOfEntryDto;
            try
            {
                var wayOfEntry = await _wayOfEntryManager.GetByRequestIdAsync(requestId);
                wayOfEntryDto = _mapper.Map<WayOfEntryDto>(wayOfEntry);
            }
            catch (EntityNotFoundException)
            {
                wayOfEntryDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = wayOfEntryDto });
        }

        [HttpGet("{requestId}/visit")]
        [HttpGet("{requestId}/links/visit")]
        public async Task<IActionResult> GetRelatedVisitByIdAsync(int requestId)
        {
            VisitDto visitDto;
            try
            {
                var visit = await _visitManager.GetByRequestIdAsync(requestId);
                visitDto = _mapper.Map<VisitDto>(visit);
            }
            catch (EntityNotFoundException)
            {
                visitDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = visitDto });
        }
    }
}
