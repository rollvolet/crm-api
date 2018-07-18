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
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class OffersController : Controller
    {
        private readonly IOfferManager _offerManager;
        private readonly ICustomerManager _customerManager;
        private readonly IContactManager _contactManager;
        private readonly IBuildingManager _buildingManager;
        private readonly IRequestManager _requestManager;
        private readonly IOrderManager _orderManager;
        private readonly IVatRateManager _vatRateManager;
        private readonly ISubmissionTypeManager _submissionTypeManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;
        private readonly ILogger _logger;

        public OffersController(IOfferManager offerManager, IRequestManager requestManager, IOrderManager orderManager,
                                    ICustomerManager customerManager, IContactManager contactManager, IBuildingManager buildingManager,
                                    IVatRateManager vatRateManager, ISubmissionTypeManager submissionTypeManager,
                                    IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder,
                                    ILogger<OffersController> logger)
        {
            _offerManager = offerManager;
            _customerManager = customerManager;
            _contactManager = contactManager;
            _buildingManager = buildingManager;
            _requestManager = requestManager;
            _orderManager = orderManager;
            _vatRateManager = vatRateManager;
            _submissionTypeManager = submissionTypeManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedOffers = await _offerManager.GetAllAsync(querySet);

            var offerDtos = _mapper.Map<IEnumerable<OfferDto>>(pagedOffers.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedOffers.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedOffers);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedOffers);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = offerDtos, Included = included });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var offer = await _offerManager.GetByIdAsync(id, querySet);

            var offerDto = _mapper.Map<OfferDto>(offer, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(offer, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = offerDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResourceRequest<OfferRequestDto> resource)
        {
            if (resource.Data.Type != "offers") return StatusCode(409);

            var offer = _mapper.Map<Offer>(resource.Data);

            offer = await _offerManager.CreateAsync(offer);
            var offerDto = _mapper.Map<OfferDto>(offer);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, offerDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = offerDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ResourceRequest<OfferRequestDto> resource)
        {
            if (resource.Data.Type != "offers" || resource.Data.Id != id) return StatusCode(409);

            var offer = _mapper.Map<Offer>(resource.Data);

            offer = await _offerManager.UpdateAsync(offer);

            var offerDto = _mapper.Map<OfferDto>(offer);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = offerDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _offerManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("{offerId}/customer")]
        [HttpGet("{offerId}/links/customer")]
        public async Task<IActionResult> GetRelatedCustomerById(int offerId)
        {
            CustomerDto customerDto;
            try
            {
                var customer = await _customerManager.GetByOfferIdAsync(offerId);
                customerDto = _mapper.Map<CustomerDto>(customer);
            }
            catch (EntityNotFoundException)
            {
                customerDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = customerDto });
        }

        [HttpGet("{offerId}/contact")]
        [HttpGet("{offerId}/links/contact")]
        public async Task<IActionResult> GetRelatedContactById(int offerId)
        {
            ContactDto contactDto;
            try
            {
                var contact = await _contactManager.GetByOfferIdAsync(offerId);
                contactDto = _mapper.Map<ContactDto>(contact);
            }
            catch (EntityNotFoundException)
            {
                contactDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = contactDto });
        }

        [HttpGet("{offerId}/building")]
        [HttpGet("{offerId}/links/building")]
        public async Task<IActionResult> GetRelatedBuildingById(int offerId)
        {
            BuildingDto buildingDto;
            try
            {
                var building = await _buildingManager.GetByOfferIdAsync(offerId);
                buildingDto = _mapper.Map<BuildingDto>(building);
            }
            catch (EntityNotFoundException)
            {
                buildingDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = buildingDto });
        }

        [HttpGet("{offerId}/request")]
        [HttpGet("{offerId}/links/request")]
        public async Task<IActionResult> GetRelatedRequestById(int offerId)
        {
            RequestDto requestDto;
            try
            {
                var request = await _requestManager.GetByOfferIdAsync(offerId);
                requestDto = _mapper.Map<RequestDto>(request);
            }
            catch (EntityNotFoundException)
            {
                requestDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = requestDto });
        }

        [HttpGet("{offerId}/order")]
        [HttpGet("{offerId}/links/order")]
        public async Task<IActionResult> GetRelatedOrderById(int offerId)
        {
            OrderDto orderDto;
            try
            {
                var order = await _orderManager.GetByOfferIdAsync(offerId);
                orderDto = _mapper.Map<OrderDto>(order);
            }
            catch (EntityNotFoundException)
            {
                orderDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = orderDto });
        }

        [HttpGet("{offerId}/vat-rate")]
        [HttpGet("{offerId}/links/vat-rate")]
        public async Task<IActionResult> GetRelatedVatRateById(int offerId)
        {
            VatRateDto vatRateDto;
            try
            {
                var vatRate = await _vatRateManager.GetByOfferIdAsync(offerId);
                vatRateDto = _mapper.Map<VatRateDto>(vatRate);
            }
            catch (EntityNotFoundException)
            {
                vatRateDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = vatRateDto });
        }

        [HttpGet("{offerId}/submission-type")]
        [HttpGet("{offerId}/links/submission-type")]
        public async Task<IActionResult> GetRelatedSubmissionTypeById(int offerId)
        {
            SubmissionTypeDto submissionTypeDto;
            try
            {
                var submissionType = await _submissionTypeManager.GetByOfferIdAsync(offerId);
                submissionTypeDto = _mapper.Map<SubmissionTypeDto>(submissionType);
            }
            catch (EntityNotFoundException)
            {
                submissionTypeDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = submissionTypeDto });
        }
    }
}