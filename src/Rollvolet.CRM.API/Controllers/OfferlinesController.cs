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
using Rollvolet.CRM.APIContracts.DTO.Offerlines;
using Rollvolet.CRM.APIContracts.DTO.Offers;
using Rollvolet.CRM.APIContracts.DTO.Orders;
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
    public class OfferlinesController : Controller
    {
        private readonly IOfferlineManager _offerlineManager;
        private readonly IOfferManager _offerManager;
        private readonly IVatRateManager _vatRateManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;
        private readonly ILogger _logger;

        public OfferlinesController(IOfferlineManager offerlineManager, IOfferManager offerManager, IVatRateManager vatRateManager,
                                    IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder,
                                    ILogger<OffersController> logger)
        {
            _offerlineManager = offerlineManager;
            _offerManager = offerManager;
            _vatRateManager = vatRateManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var offerline = await _offerlineManager.GetByIdAsync(id, querySet);

            var offerlineDto = _mapper.Map<OfferlineDto>(offerline, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(offerline, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = offerlineDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<OfferlineRequestDto> resource)
        {
            if (resource.Data.Type != "offerlines") return StatusCode(409);

            var offerline = _mapper.Map<Offerline>(resource.Data);

            offerline = await _offerlineManager.CreateAsync(offerline);
            var offerlineDto = _mapper.Map<OfferlineDto>(offerline);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, offerlineDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = offerlineDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ResourceRequest<OfferlineRequestDto> resource)
        {
            if (resource.Data.Type != "offerlines" || resource.Data.Id != id) return StatusCode(409);

            var offerline = _mapper.Map<Offerline>(resource.Data);

            offerline = await _offerlineManager.UpdateAsync(offerline);

            var offerlineDto = _mapper.Map<OfferlineDto>(offerline);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = offerlineDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _offerlineManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("{offerlineId}/offer")]
        [HttpGet("{offerlineId}/links/offer")]
        public async Task<IActionResult> GetRelatedOfferByIdAsync(int offerlineId)
        {
            OfferDto offerDto;
            try
            {
                var offer = await _offerManager.GetByOfferlineIdAsync(offerlineId);
                offerDto = _mapper.Map<OfferDto>(offer);
            }
            catch (EntityNotFoundException)
            {
                offerDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = offerDto });
        }

        [HttpGet("{offerlineId}/vat-rate")]
        [HttpGet("{offerlineId}/links/vat-rate")]
        public async Task<IActionResult> GetRelatedVatRateByIdAsync(int offerlineId)
        {
            VatRateDto vatRateDto;
            try
            {
                var vatRate = await _vatRateManager.GetByOfferlineIdAsync(offerlineId);
                vatRateDto = _mapper.Map<VatRateDto>(vatRate);
            }
            catch (EntityNotFoundException)
            {
                vatRateDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = vatRateDto });
        }
    }
}