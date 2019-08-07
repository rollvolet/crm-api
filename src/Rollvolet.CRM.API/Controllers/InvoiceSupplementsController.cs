using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.APIContracts.DTO.InvoiceSupplements;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("invoice-supplements")]
    [Authorize]
    public class InvoiceSupplementsController : ControllerBase
    {
        private readonly IInvoiceSupplementManager _invoiceSupplementManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public InvoiceSupplementsController(IInvoiceSupplementManager invoiceSupplementManager,
                                    IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _invoiceSupplementManager = invoiceSupplementManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<InvoiceSupplementRequestDto> resource)
        {
            if (resource.Data.Type != "invoice-supplements") return StatusCode(409);

            var invoiceSupplement = _mapper.Map<InvoiceSupplement>(resource.Data);

            invoiceSupplement = await _invoiceSupplementManager.CreateAsync(invoiceSupplement);
            var invoiceSupplementDto = _mapper.Map<InvoiceSupplementDto>(invoiceSupplement);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, invoiceSupplementDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = invoiceSupplementDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ResourceRequest<InvoiceSupplementRequestDto> resource)
        {
            if (resource.Data.Type != "invoice-supplements" || resource.Data.Id != id) return StatusCode(409);

            var invoiceSupplement = _mapper.Map<InvoiceSupplement>(resource.Data);

            invoiceSupplement = await _invoiceSupplementManager.UpdateAsync(invoiceSupplement);

            var invoiceSupplementDto = _mapper.Map<InvoiceSupplementDto>(invoiceSupplement);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = invoiceSupplementDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _invoiceSupplementManager.DeleteAsync(id);

            return NoContent();
        }
    }
}