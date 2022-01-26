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
using Rollvolet.CRM.APIContracts.DTO.Deposits;
using Rollvolet.CRM.APIContracts.DTO.Interventions;
using Rollvolet.CRM.APIContracts.DTO.Invoices;
using Rollvolet.CRM.APIContracts.DTO.Orders;
using Rollvolet.CRM.APIContracts.DTO.WorkingHours;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceManager _invoiceManager;
        private readonly ICustomerManager _customerManager;
        private readonly IContactManager _contactManager;
        private readonly IBuildingManager _buildingManager;
        private readonly IOrderManager _orderManager;
        private readonly IInterventionManager _interventionManager;
        private readonly IVatRateManager _vatRateManager;
        private readonly IWorkingHourManager _workingHourManager;
        private readonly IDepositManager _depositManager;
        private readonly IDepositInvoiceManager _depositInvoiceManager;
        private readonly IDocumentGenerationManager _documentGenerationManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public InvoicesController(IInvoiceManager invoiceManager, ICustomerManager customerManager, IContactManager contactManager,
                                    IBuildingManager buildingManager, IOrderManager orderManager, IInterventionManager interventionManager,
                                    IVatRateManager vatRateManager, IDepositManager depositManager, IDepositInvoiceManager depositInvoiceManager,
                                    IWorkingHourManager workingHourManager, IDocumentGenerationManager documentGenerationManager,
                                    IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _invoiceManager = invoiceManager;
            _customerManager = customerManager;
            _contactManager = contactManager;
            _buildingManager = buildingManager;
            _orderManager = orderManager;
            _interventionManager = interventionManager;
            _vatRateManager = vatRateManager;
            _depositManager = depositManager;
            _depositInvoiceManager = depositInvoiceManager;
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

        [HttpGet("{invoiceId}/intervention")]
        [HttpGet("{invoiceId}/links/intervention")]
        public async Task<IActionResult> GetRelatedInterventionByIdAsync(int invoiceId)
        {
            InterventionDto interventionDto;
            try
            {
                var intervention = await _interventionManager.GetByInvoiceIdAsync(invoiceId);
                interventionDto = _mapper.Map<InterventionDto>(intervention);
            }
            catch (EntityNotFoundException)
            {
                interventionDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = interventionDto });
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
    }
}