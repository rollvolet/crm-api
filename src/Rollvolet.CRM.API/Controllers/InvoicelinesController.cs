using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.DTO.Invoicelines;
using Rollvolet.CRM.APIContracts.DTO.Invoices;
using Rollvolet.CRM.APIContracts.DTO.Orders;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class InvoicelinesController : ControllerBase
    {
        private readonly IInvoicelineManager _invoicelineManager;
        private readonly IOrderManager _orderManager;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IVatRateManager _vatRateManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;
        private readonly ILogger _logger;

        public InvoicelinesController(IInvoicelineManager invoicelineManager, IOrderManager orderManager, IInvoiceManager invoiceManager, IVatRateManager vatRateManager,
                                    IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder,
                                    ILogger<OffersController> logger)
        {
            _invoicelineManager = invoicelineManager;
            _orderManager = orderManager;
            _invoiceManager = invoiceManager;
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

            var invoiceline = await _invoicelineManager.GetByIdAsync(id, querySet);

            var invoicelineDto = _mapper.Map<InvoicelineDto>(invoiceline, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(invoiceline, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = invoicelineDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<InvoicelineRequestDto> resource)
        {
            if (resource.Data.Type != "invoicelines") return StatusCode(409);

            var invoiceline = _mapper.Map<Invoiceline>(resource.Data);

            invoiceline = await _invoicelineManager.CreateAsync(invoiceline);
            var invoicelineDto = _mapper.Map<InvoicelineDto>(invoiceline);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, invoicelineDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = invoicelineDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ResourceRequest<InvoicelineRequestDto> resource)
        {
            if (resource.Data.Type != "invoicelines" || resource.Data.Id != id) return StatusCode(409);

            var invoiceline = _mapper.Map<Invoiceline>(resource.Data);

            invoiceline = await _invoicelineManager.UpdateAsync(invoiceline);

            var invoicelineDto = _mapper.Map<InvoicelineDto>(invoiceline);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = invoicelineDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _invoicelineManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("{invoicelineId}/order")]
        [HttpGet("{invoicelineId}/links/order")]
        public async Task<IActionResult> GetRelatedOrderByIdAsync(int invoicelineId)
        {
            OrderDto orderDto;
            try
            {
                var order = await _orderManager.GetByInvoicelineIdAsync(invoicelineId);
                orderDto = _mapper.Map<OrderDto>(order);
            }
            catch (EntityNotFoundException)
            {
                orderDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = orderDto });
        }

        [HttpGet("{invoicelineId}/invoice")]
        [HttpGet("{invoicelineId}/links/invoice")]
        public async Task<IActionResult> GetRelatedInvoiceByIdAsync(int invoicelineId)
        {
            InvoiceDto invoiceDto;
            try
            {
                var invoice = await _invoiceManager.GetByInvoicelineIdAsync(invoicelineId);
                invoiceDto = _mapper.Map<InvoiceDto>(invoice);
            }
            catch (EntityNotFoundException)
            {
                invoiceDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = invoiceDto });
        }

        [HttpGet("{invoicelineId}/vat-rate")]
        [HttpGet("{invoicelineId}/links/vat-rate")]
        public async Task<IActionResult> GetRelatedVatRateByIdAsync(int invoicelineId)
        {
            VatRateDto vatRateDto;
            try
            {
                var vatRate = await _vatRateManager.GetByInvoicelineIdAsync(invoicelineId);
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