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
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class InvoicesController : Controller
    {
        private readonly IInvoiceManager _invoiceManager;
        private readonly IWorkingHourManager _workingHourManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public InvoicesController(IInvoiceManager invoiceManager, IWorkingHourManager workingHourManager, IIncludedCollector includedCollector, 
                                    IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _invoiceManager = invoiceManager;
            _workingHourManager = workingHourManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedInvoices = await _invoiceManager.GetAllAsync(querySet);

            var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(pagedInvoices.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedInvoices.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedInvoices);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedInvoices);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = invoiceDtos, Included = included });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var invoice = await _invoiceManager.GetByIdAsync(id, querySet);

            var orderDto = _mapper.Map<InvoiceDto>(invoice, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(invoice, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = orderDto, Included = included });
        }   

        [HttpGet("{invoiceId}/working-hours")]
        [HttpGet("{invoiceId}/links/working-hours")]
        public async Task<IActionResult> GetRelatedWorkingHoursByInvoiceId(int invoiceId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedWorkingHours = await _workingHourManager.GetAllByInvoiceIdAsync(invoiceId, querySet);

            var workingHourDtos = _mapper.Map<IEnumerable<WorkingHourDto>>(pagedWorkingHours.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedWorkingHours.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedWorkingHours);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedWorkingHours);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = workingHourDtos, Included = included });
        }  
    }
} 