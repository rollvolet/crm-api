using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Rollvolet.CRM.APIContracts.DTO.Interventions;
using Rollvolet.CRM.APIContracts.DTO.Invoicelines;
using Rollvolet.CRM.APIContracts.DTO.Invoices;
using Rollvolet.CRM.APIContracts.DTO.Offers;
using Rollvolet.CRM.APIContracts.DTO.Orders;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderManager _orderManager;
        private readonly ICustomerManager _customerManager;
        private readonly IContactManager _contactManager;
        private readonly IBuildingManager _buildingManager;
        private readonly IOfferManager _offerManager;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IDepositManager _depositManager;
        private readonly IDepositInvoiceManager _depositInvoiceManager;
        private readonly IVatRateManager _vatRateManager;
        private readonly IInterventionManager _interventionManager;
        private readonly IInvoicelineManager _invoicelineManager;
        private readonly IEmployeeManager _employeeManager;
        private readonly IDocumentGenerationManager _documentGenerationManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public OrdersController(IOrderManager orderManager, IOfferManager offerManager, IInvoiceManager invoiceManager,
                                ICustomerManager customerManager, IContactManager contactManager, IBuildingManager buildingManager,
                                IDepositManager depositManager, IDepositInvoiceManager depositInvoiceManager, IVatRateManager vatRateManager,
                                IInterventionManager interventionManager, IInvoicelineManager invoicelineManager,
                                IEmployeeManager employeeManager, IDocumentGenerationManager documentGenerationManager,
                                IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _orderManager = orderManager;
            _customerManager = customerManager;
            _contactManager = contactManager;
            _buildingManager = buildingManager;
            _offerManager = offerManager;
            _invoiceManager = invoiceManager;
            _depositManager = depositManager;
            _depositInvoiceManager = depositInvoiceManager;
            _vatRateManager = vatRateManager;
            _interventionManager = interventionManager;
            _invoicelineManager = invoicelineManager;
            _employeeManager = employeeManager;
            _documentGenerationManager = documentGenerationManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedOrders = await _orderManager.GetAllAsync(querySet);

            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(pagedOrders.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedOrders.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedOrders);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedOrders);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = orderDtos, Included = included });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var order = await _orderManager.GetByIdAsync(id, querySet);

            var orderDto = _mapper.Map<OrderDto>(order, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(order, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = orderDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<OrderRequestDto> resource)
        {
            if (resource.Data.Type != "orders") return StatusCode(409);

            var order = _mapper.Map<Order>(resource.Data);

            order = await _orderManager.CreateAsync(order);
            var orderDto = _mapper.Map<OrderDto>(order);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, orderDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = orderDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ResourceRequest<OrderRequestDto> resource)
        {
            if (resource.Data.Type != "orders" || resource.Data.Id != id) return StatusCode(409);

            var order = _mapper.Map<Order>(resource.Data);

            order = await _orderManager.UpdateAsync(order);

            var orderDto = _mapper.Map<OrderDto>(order);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = orderDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _orderManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpPut("{orderId}/planning-event")]
        public async Task<IActionResult> UpdatePlanningEventAsync(int orderId)
        {
            await _orderManager.SyncPlanningEventAsync(orderId, true);
            return NoContent();
        }

        [HttpPost("{id}/documents")]
        public async Task<IActionResult> CreateOrderDocumentAsync(int id)
        {
            await _documentGenerationManager.CreateAndStoreOrderDocumentAsync(id);
            // TODO return download location in Location header
            return NoContent();
        }

        [HttpPost("{id}/delivery-notes")]
        public async Task<IActionResult> CreateDeliveryNoteAsync(int id)
        {
            await _documentGenerationManager.CreateAndStoreDeliveryNoteAsync(id);
            // TODO return download location in Location header
            return NoContent();
        }

        [HttpPost("{id}/production-tickets")]
        public async Task<IActionResult> CreateProductionTicketAsync(int id)
        {
            await _documentGenerationManager.CreateAndStoreProductionTicketTemplateAsync(id);
            // TODO return download location in Location header
            return NoContent();
        }

        [HttpPost("{orderId}/production-ticket")]
        public async Task<IActionResult> UploadProductionTicketAsync(int orderId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new IllegalArgumentException("InvalidFile", "File cannot be empty");

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                await _documentGenerationManager.UploadProductionTicketAsync(orderId, stream);
            }

            return NoContent();
        }

        [HttpDelete("{orderId}/production-ticket")]
        public async Task<IActionResult> DeleteProductionTicketAsync(int orderId)
        {
            await _documentGenerationManager.DeleteProductionTicketAsync(orderId);

            return NoContent();
        }

        [HttpGet("{orderId}/customer")]
        [HttpGet("{orderId}/links/customer")]
        public async Task<IActionResult> GetRelatedCustomerByIdAsync(int orderId)
        {
            CustomerDto customerDto;
            try
            {
                var customer = await _customerManager.GetByOrderIdAsync(orderId);
                customerDto = _mapper.Map<CustomerDto>(customer);
            }
            catch (EntityNotFoundException)
            {
                customerDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = customerDto });
        }

        [HttpGet("{orderId}/contact")]
        [HttpGet("{orderId}/links/contact")]
        public async Task<IActionResult> GetRelatedContactByIdAsync(int orderId)
        {
            ContactDto contactDto;
            try
            {
                var contact = await _contactManager.GetByOrderIdAsync(orderId);
                contactDto = _mapper.Map<ContactDto>(contact);
            }
            catch (EntityNotFoundException)
            {
                contactDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = contactDto });
        }

        [HttpGet("{orderId}/building")]
        [HttpGet("{orderId}/links/building")]
        public async Task<IActionResult> GetRelatedBuildingByIdAsync(int orderId)
        {
            BuildingDto buildingDto;
            try
            {
                var building = await _buildingManager.GetByOrderIdAsync(orderId);
                buildingDto = _mapper.Map<BuildingDto>(building);
            }
            catch (EntityNotFoundException)
            {
                buildingDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = buildingDto });
        }

        [HttpGet("{orderId}/offer")]
        [HttpGet("{orderId}/links/offer")]
        public async Task<IActionResult> GetRelatedOfferByIdAsync(int orderId)
        {
            OfferDto offerDto;
            try
            {
                var offer = await _offerManager.GetByOrderIdAsync(orderId);
                offerDto = _mapper.Map<OfferDto>(offer);
            }
            catch (EntityNotFoundException)
            {
                offerDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = offerDto });
        }

        [HttpGet("{orderId}/invoice")]
        [HttpGet("{orderId}/links/invoice")]
        public async Task<IActionResult> GetRelatedInvoiceByIdAsync(int orderId)
        {
            InvoiceDto invoiceDto;
            try
            {
                var invoice = await _invoiceManager.GetByOrderIdAsync(orderId);
                invoiceDto = _mapper.Map<InvoiceDto>(invoice);
            }
            catch (EntityNotFoundException)
            {
                invoiceDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = invoiceDto });
        }

        [HttpGet("{orderId}/deposits")]
        [HttpGet("{orderId}/links/deposits")]
        public async Task<IActionResult> GetRelatedDepositsByOrderIdAsync(int orderId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);
            querySet.Include.Fields = querySet.Include.Fields.Concat(new string[] { "payment" }).ToArray();

            var pagedDeposits = await _depositManager.GetAllByOrderIdAsync(orderId, querySet);

            var depositDtos = _mapper.Map<IEnumerable<DepositDto>>(pagedDeposits.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedDeposits.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedDeposits);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedDeposits);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = depositDtos, Included = included });
        }

        [HttpGet("{orderId}/deposit-invoices")]
        [HttpGet("{orderId}/links/deposit-invoices")]
        public async Task<IActionResult> GetRelatedDepositInvoicesByOrderIdAsync(int orderId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedInvoices = await _depositInvoiceManager.GetAllByOrderIdAsync(orderId, querySet);

            var depositInvoiceDtos = _mapper.Map<IEnumerable<DepositInvoiceDto>>(pagedInvoices.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedInvoices.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedInvoices);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedInvoices);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = depositInvoiceDtos, Included = included });
        }

        [HttpGet("{orderId}/vat-rate")]
        [HttpGet("{orderId}/links/vat-rate")]
        public async Task<IActionResult> GetRelatedVatRateByIdAsync(int orderId)
        {
            VatRateDto vatRateDto;
            try
            {
                var vatRate = await _vatRateManager.GetByOrderIdAsync(orderId);
                vatRateDto = _mapper.Map<VatRateDto>(vatRate);
            }
            catch (EntityNotFoundException)
            {
                vatRateDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = vatRateDto });
        }

        [HttpGet("{orderId}/interventions")]
        [HttpGet("{orderId}/links/interventions")]
        public async Task<IActionResult> GetRelatedInterventionsByOrderIdAsync(int orderId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedInterventions = await _interventionManager.GetAllByOrderIdAsync(orderId, querySet);

            var interventionDtos = _mapper.Map<IEnumerable<InterventionDto>>(pagedInterventions.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedInterventions.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedInterventions);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedInterventions);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = interventionDtos, Included = included });
        }

        [HttpGet("{orderId}/invoicelines")]
        [HttpGet("{orderId}/links/invoicelines")]
        public async Task<IActionResult> GetRelatedInvoicelinesByOrderIdAsync(int orderId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);
            querySet.Include.Fields = querySet.Include.Fields.Concat(new string[] { "vat-rate" }).ToArray();

            var pagedInvoicelines = await _invoicelineManager.GetAllByOrderIdAsync(orderId, querySet);

            var invoicelineDtos = _mapper.Map<IEnumerable<InvoicelineDto>>(pagedInvoicelines.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedInvoicelines.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedInvoicelines);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedInvoicelines);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = invoicelineDtos, Included = included });
        }

        [HttpGet("{orderId}/technicians")]
        [HttpGet("{orderId}/links/technicians")]
        public async Task<IActionResult> GetRelatedTechniciansByIdAsync(int orderId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedEmployees = await _employeeManager.GetAllByOrderIdAsync(orderId, querySet);

            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(pagedEmployees.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedEmployees.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedEmployees);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedEmployees);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = employeeDtos, Included = included });
        }
    }
}