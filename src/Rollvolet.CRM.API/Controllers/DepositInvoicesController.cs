using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.DTO.Buildings;
using Rollvolet.CRM.APIContracts.DTO.Contacts;
using Rollvolet.CRM.APIContracts.DTO.Customers;
using Rollvolet.CRM.APIContracts.DTO.DepositInvoices;
using Rollvolet.CRM.APIContracts.DTO.Orders;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("deposit-invoices")]
    public class DepositInvoicesController : ControllerBase
    {
        private readonly IDepositInvoiceManager _depositInvoiceManager;
        private readonly ICustomerManager _customerManager;
        private readonly IContactManager _contactManager;
        private readonly IBuildingManager _buildingManager;
        private readonly IOrderManager _orderManager;
        private readonly IVatRateManager _vatRateManager;
        private readonly IDocumentGenerationManager _documentGenerationManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public DepositInvoicesController(IDepositInvoiceManager depositInvoiceManager, ICustomerManager customerManager,
                                    IContactManager contactManager, IBuildingManager buildingManager, IOrderManager orderManager,
                                    IVatRateManager vatRateManager, IDocumentGenerationManager documentGenerationManager,
                                    IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _depositInvoiceManager = depositInvoiceManager;
            _customerManager = customerManager;
            _contactManager = contactManager;
            _buildingManager = buildingManager;
            _orderManager = orderManager;
            _vatRateManager = vatRateManager;
            _documentGenerationManager = documentGenerationManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedInvoices = await _depositInvoiceManager.GetAllAsync(querySet);

            var invoiceDtos = _mapper.Map<IEnumerable<DepositInvoiceDto>>(pagedInvoices.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedInvoices.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedInvoices);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedInvoices);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = invoiceDtos, Included = included });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var invoice = await _depositInvoiceManager.GetByIdAsync(id, querySet);

            var orderDto = _mapper.Map<DepositInvoiceDto>(invoice, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(invoice, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = orderDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<DepositInvoiceRequestDto> resource)
        {
            if (resource.Data.Type != "deposit-invoices") return StatusCode(409);

            var depositInvoice = _mapper.Map<DepositInvoice>(resource.Data);

            depositInvoice = await _depositInvoiceManager.CreateAsync(depositInvoice);
            var depositInvoiceDto = _mapper.Map<DepositInvoiceDto>(depositInvoice);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, depositInvoiceDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = depositInvoiceDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ResourceRequest<DepositInvoiceRequestDto> resource)
        {
            if (resource.Data.Type != "deposit-invoices" || resource.Data.Id != id) return StatusCode(409);

            var depositInvoice = _mapper.Map<DepositInvoice>(resource.Data);

            depositInvoice = await _depositInvoiceManager.UpdateAsync(depositInvoice);

            var depositInvoiceDto = _mapper.Map<DepositInvoiceDto>(depositInvoice);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = depositInvoiceDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _depositInvoiceManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpPost("{id}/documents")]
        public async Task<IActionResult> CreateDepositInvoiceDocumentAsync(int id)
        {
            await _documentGenerationManager.CreateAndStoreDepositInvoiceDocumentAsync(id);
            // TODO return download location in Location header
            return NoContent();
        }

        [HttpPost("{invoiceId}/certificates")]
        public async Task<IActionResult> CreateCertificateAsync(int invoiceId)
        {
            await _documentGenerationManager.CreateCertificateTemplateForDepositInvoiceAsync(invoiceId);
            // TODO return download location in Location header
            return NoContent();
        }

        [HttpPost("{invoiceId}/certificate")]
        public async Task<IActionResult> UploadCertificateAsync(int invoiceId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new IllegalArgumentException("InvalidFile", "File cannot be empty");

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                await _documentGenerationManager.UploadCertificateForDepositInvoiceAsync(invoiceId, stream, file.FileName);
            }

            return NoContent();
        }

        [HttpPost("{invoiceId}/certificate-recyclations")]
        public async Task<IActionResult> RecycleCertificateAsync(int invoiceId, [FromBody] CertificateRecyclationDto body)
        {
            var sourceId = Int32.Parse(body.Id);
            var isDeposit = body.Type == "deposit-invoices"; // type of the source invoice
            await _documentGenerationManager.RecycleCertificateForDepositInvoiceAsync(invoiceId, sourceId, isDeposit);
            // TODO return download location in Location header
            return NoContent();
        }

        [HttpDelete("{invoiceId}/certificate")]
        public async Task<IActionResult> DeleteCertificateAsync(int invoiceId)
        {
            await _documentGenerationManager.DeleteCertificateForDepositInvoiceAsync(invoiceId);

            return NoContent();
        }

        [HttpGet("{invoiceId}/customer")]
        [HttpGet("{invoiceId}/links/customer")]
        public async Task<IActionResult> GetRelatedCustomerByIdAsync(int invoiceId)
        {
            CustomerDto customerDto;
            try
            {
                var customer = await _customerManager.GetByDepositInvoiceIdAsync(invoiceId);
                customerDto = _mapper.Map<CustomerDto>(customer);
            }
            catch (EntityNotFoundException)
            {
                customerDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = customerDto });
        }

        [HttpGet("{invoiceId}/contact")]
        [HttpGet("{invoiceId}/links/contact")]
        public async Task<IActionResult> GetRelatedContactByIdAsync(int invoiceId)
        {
            ContactDto contactDto;
            try
            {
                var contact = await _contactManager.GetByDepositInvoiceIdAsync(invoiceId);
                contactDto = _mapper.Map<ContactDto>(contact);
            }
            catch (EntityNotFoundException)
            {
                contactDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = contactDto });
        }

        [HttpGet("{invoiceId}/building")]
        [HttpGet("{invoiceId}/links/building")]
        public async Task<IActionResult> GetRelatedBuildingByIdAsync(int invoiceId)
        {
            BuildingDto buildingDto;
            try
            {
                var building = await _buildingManager.GetByDepositInvoiceIdAsync(invoiceId);
                buildingDto = _mapper.Map<BuildingDto>(building);
            }
            catch (EntityNotFoundException)
            {
                buildingDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = buildingDto });
        }

        [HttpGet("{invoiceId}/order")]
        [HttpGet("{invoiceId}/links/order")]
        public async Task<IActionResult> GetRelatedOrderByIdAsync(int invoiceId)
        {
            OrderDto orderDto;
            try
            {
                var order = await _orderManager.GetByDepositInvoiceIdAsync(invoiceId);
                orderDto = _mapper.Map<OrderDto>(order);
            }
            catch (EntityNotFoundException)
            {
                orderDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = orderDto });
        }

        [HttpGet("{invoiceId}/vat-rate")]
        [HttpGet("{invoiceId}/links/vat-rate")]
        public async Task<IActionResult> GetRelatedVatRateByIdAsync(int invoiceId)
        {
            VatRateDto vatRateDto;
            try
            {
                var vatRate = await _vatRateManager.GetByDepositInvoiceIdAsync(invoiceId);
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