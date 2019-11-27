using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.DTO.Buildings;
using Rollvolet.CRM.APIContracts.DTO.Contacts;
using Rollvolet.CRM.APIContracts.DTO.Customers;
using Rollvolet.CRM.APIContracts.DTO.DepositInvoices;
using Rollvolet.CRM.APIContracts.DTO.Deposits;
using Rollvolet.CRM.APIContracts.DTO.Invoices;
using Rollvolet.CRM.APIContracts.DTO.InvoiceSupplements;
using Rollvolet.CRM.APIContracts.DTO.Orders;
using Rollvolet.CRM.APIContracts.DTO.WorkingHours;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceManager _invoiceManager;
        private readonly ICustomerManager _customerManager;
        private readonly IContactManager _contactManager;
        private readonly IBuildingManager _buildingManager;
        private readonly IOrderManager _orderManager;
        private readonly IVatRateManager _vatRateManager;
        private readonly IWorkingHourManager _workingHourManager;
        private readonly IDepositManager _depositManager;
        private readonly IDepositInvoiceManager _depositInvoiceManager;
        private readonly IInvoiceSupplementManager _invoiceSupplementManager;
        private readonly IDocumentGenerationManager _documentGenerationManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public InvoicesController(IInvoiceManager invoiceManager, ICustomerManager customerManager, IContactManager contactManager,
                                    IBuildingManager buildingManager, IOrderManager orderManager, IVatRateManager vatRateManager,
                                    IDepositManager depositManager, IDepositInvoiceManager depositInvoiceManager,
                                    IInvoiceSupplementManager invoiceSupplementManager, IWorkingHourManager workingHourManager,
                                    IDocumentGenerationManager documentGenerationManager,
                                    IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _invoiceManager = invoiceManager;
            _customerManager = customerManager;
            _contactManager = contactManager;
            _buildingManager = buildingManager;
            _orderManager = orderManager;
            _vatRateManager = vatRateManager;
            _depositManager = depositManager;
            _depositInvoiceManager = depositInvoiceManager;
            _invoiceSupplementManager = invoiceSupplementManager;
            _workingHourManager = workingHourManager;
            _documentGenerationManager = documentGenerationManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
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
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var invoice = await _invoiceManager.GetByIdAsync(id, querySet);

            var orderDto = _mapper.Map<InvoiceDto>(invoice, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(invoice, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = orderDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<InvoiceRequestDto> resource)
        {
            if (resource.Data.Type != "invoices") return StatusCode(409);

            var invoice = _mapper.Map<Invoice>(resource.Data);

            invoice = await _invoiceManager.CreateAsync(invoice);
            var invoiceDto = _mapper.Map<InvoiceDto>(invoice);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, invoiceDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = invoiceDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ResourceRequest<InvoiceRequestDto> resource)
        {
            if (resource.Data.Type != "invoices" || resource.Data.Id != id) return StatusCode(409);

            var invoice = _mapper.Map<Invoice>(resource.Data);

            invoice = await _invoiceManager.UpdateAsync(invoice);

            var invoiceDto = _mapper.Map<InvoiceDto>(invoice);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = invoiceDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _invoiceManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpPost("{id}/documents")]
        public async Task<IActionResult> CreateInvoiceDocumentAsync(int id)
        {
            await _documentGenerationManager.CreateAndStoreInvoiceDocumentAsync(id);
            // TODO return download location in Location header
            return NoContent();
        }

        [HttpPost("{invoiceId}/certificates")]
        public async Task<IActionResult> CreateCertificateAsync(int invoiceId)
        {
            await _documentGenerationManager.CreateCertificateTemplateForInvoiceAsync(invoiceId);
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
                await _documentGenerationManager.UploadCertificateForInvoiceAsync(invoiceId, stream, file.FileName);
            }

            return NoContent();
        }

        [HttpDelete("{invoiceId}/certificate")]
        public async Task<IActionResult> DeleteCertificateAsync(int invoiceId)
        {
            await _documentGenerationManager.DeleteCertificateForInvoiceAsync(invoiceId);

            return NoContent();
        }

        [HttpGet("{invoiceId}/customer")]
        [HttpGet("{invoiceId}/links/customer")]
        public async Task<IActionResult> GetRelatedCustomerByIdAsync(int invoiceId)
        {
            CustomerDto customerDto;
            try
            {
                var customer = await _customerManager.GetByInvoiceIdAsync(invoiceId);
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
                var contact = await _contactManager.GetByInvoiceIdAsync(invoiceId);
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
                var building = await _buildingManager.GetByInvoiceIdAsync(invoiceId);
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
                var order = await _orderManager.GetByInvoiceIdAsync(invoiceId);
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
                var vatRate = await _vatRateManager.GetByInvoiceIdAsync(invoiceId);
                vatRateDto = _mapper.Map<VatRateDto>(vatRate);
            }
            catch (EntityNotFoundException)
            {
                vatRateDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = vatRateDto });
        }

        [HttpGet("{invoiceId}/working-hours")]
        [HttpGet("{invoiceId}/links/working-hours")]
        public async Task<IActionResult> GetRelatedWorkingHoursByInvoiceIdAsync(int invoiceId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedWorkingHours = await _workingHourManager.GetAllByInvoiceIdAsync(invoiceId, querySet);

            var workingHourDtos = _mapper.Map<IEnumerable<WorkingHourDto>>(pagedWorkingHours.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedWorkingHours.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedWorkingHours);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedWorkingHours);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = workingHourDtos, Included = included });
        }

        [HttpGet("{invoiceId}/deposits")]
        [HttpGet("{invoiceId}/links/deposits")]
        public async Task<IActionResult> GetRelatedDepositsByInvoiceIdAsync(int invoiceId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedDeposits = await _depositManager.GetAllByInvoiceIdAsync(invoiceId, querySet);

            var depositDtos = _mapper.Map<IEnumerable<DepositDto>>(pagedDeposits.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedDeposits.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedDeposits);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedDeposits);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = depositDtos, Included = included });
        }

        [HttpGet("{invoiceId}/deposit-invoices")]
        [HttpGet("{invoiceId}/links/deposit-invoices")]
        public async Task<IActionResult> GetRelatedDepositInvoicesByInvoiceIdAsync(int invoiceId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedDepositInvoices = await _depositInvoiceManager.GetAllByInvoiceIdAsync(invoiceId, querySet);

            var depositInvoiceDtos = _mapper.Map<IEnumerable<DepositInvoiceDto>>(pagedDepositInvoices.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedDepositInvoices.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedDepositInvoices);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedDepositInvoices);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = depositInvoiceDtos, Included = included });
        }

        [HttpGet("{invoiceId}/supplements")]
        [HttpGet("{invoiceId}/links/supplements")]
        public async Task<IActionResult> GetRelatedSupplementsByInvoiceIdAsync(int invoiceId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedSupplements = await _invoiceSupplementManager.GetAllByInvoiceIdAsync(invoiceId, querySet);

            var supplementDtos = _mapper.Map<IEnumerable<InvoiceSupplementDto>>(pagedSupplements.Items, o => o.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedSupplements.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedSupplements);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedSupplements);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = supplementDtos, Included = included });
        }
    }
}