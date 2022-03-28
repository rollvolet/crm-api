using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.DTO.Buildings;
using Rollvolet.CRM.APIContracts.DTO.Contacts;
using Rollvolet.CRM.APIContracts.DTO.Customers;
using Rollvolet.CRM.APIContracts.DTO.Requests;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.APIContracts.DTO.Interventions;
using Rollvolet.CRM.APIContracts.DTO.Invoices;
using Microsoft.AspNetCore.Http;
using Rollvolet.CRM.APIContracts.DTO.PlanningEvents;
using Rollvolet.CRM.APIContracts.DTO.Orders;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InterventionsController : ControllerBase
    {
        private readonly IInterventionManager _interventionManager;
        private readonly IWayOfEntryManager _wayOfEntryManager;
        private readonly ICustomerManager _customerManager;
        private readonly IContactManager _contactManager;
        private readonly IBuildingManager _buildingManager;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IOrderManager _orderManager;
        private readonly IRequestManager _requestManager;
        private readonly IEmployeeManager _employeeManager;
        private readonly IPlanningEventManager _planningEventManager;
        private readonly IDocumentGenerationManager _documentGenerationManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public InterventionsController(IInterventionManager interventionManager, IWayOfEntryManager wayOfEntryManager, IEmployeeManager employeeManager,
                                    ICustomerManager customerManager, IContactManager contactManager, IBuildingManager buildingManager,
                                    IInvoiceManager invoiceManager, IOrderManager orderManager, IRequestManager requestManager,
                                    IPlanningEventManager planningEventManager, IDocumentGenerationManager documentGenerationManager,
                                    IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _interventionManager = interventionManager;
            _wayOfEntryManager = wayOfEntryManager;
            _customerManager = customerManager;
            _contactManager = contactManager;
            _buildingManager = buildingManager;
            _invoiceManager = invoiceManager;
            _orderManager = orderManager;
            _requestManager = requestManager;
            _employeeManager = employeeManager;
            _planningEventManager = planningEventManager;
            _documentGenerationManager = documentGenerationManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedInterventions = await _interventionManager.GetAllAsync(querySet);

            var interventionDtos = _mapper.Map<IEnumerable<InterventionDto>>(pagedInterventions.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedInterventions.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedInterventions);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedInterventions);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = interventionDtos, Included = included });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var intervention = await _interventionManager.GetByIdAsync(id, querySet);

            var interventionDto = _mapper.Map<InterventionDto>(intervention, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(intervention, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = interventionDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<InterventionRequestDto> resource)
        {
            if (resource.Data.Type != "interventions") return StatusCode(409);

            var intervention = _mapper.Map<Intervention>(resource.Data);

            intervention = await _interventionManager.CreateAsync(intervention);
            var interventionDto = _mapper.Map<InterventionDto>(intervention);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, interventionDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = interventionDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ResourceRequest<InterventionRequestDto> resource)
        {
            if (resource.Data.Type != "interventions" || resource.Data.Id != id) return StatusCode(409);

            var intervention = _mapper.Map<Intervention>(resource.Data);

            intervention = await _interventionManager.UpdateAsync(intervention);

            var interventionDto = _mapper.Map<InterventionDto>(intervention);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = interventionDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _interventionManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpPost("{id}/reports")]
        public async Task<IActionResult> CreateInterventionReportAsync(int id)
        {
            await _documentGenerationManager.CreateAndStoreInterventionReportAsync(id);
            // TODO return download location in Location header
            return NoContent();
        }

        [HttpGet("{interventionId}/customer")]
        [HttpGet("{interventionId}/links/customer")]
        public async Task<IActionResult> GetRelatedCustomerByIdAsync(int interventionId)
        {
            CustomerDto customerDto;
            try
            {
                var customer = await _customerManager.GetByInterventionIdAsync(interventionId);
                customerDto = _mapper.Map<CustomerDto>(customer);
            }
            catch (EntityNotFoundException)
            {
                customerDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = customerDto });
        }

        [HttpGet("{interventionId}/contact")]
        [HttpGet("{interventionId}/links/contact")]
        public async Task<IActionResult> GetRelatedContactByIdAsync(int interventionId)
        {
            ContactDto contactDto;
            try
            {
                var contact = await _contactManager.GetByInterventionIdAsync(interventionId);
                contactDto = _mapper.Map<ContactDto>(contact);
            }
            catch (EntityNotFoundException)
            {
                contactDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = contactDto });
        }

        [HttpGet("{interventionId}/building")]
        [HttpGet("{interventionId}/links/building")]
        public async Task<IActionResult> GetRelatedBuildingByIdAsync(int interventionId)
        {
            BuildingDto buildingDto;
            try
            {
                var building = await _buildingManager.GetByInterventionIdAsync(interventionId);
                buildingDto = _mapper.Map<BuildingDto>(building);
            }
            catch (EntityNotFoundException)
            {
                buildingDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = buildingDto });
        }

        [HttpGet("{interventionId}/invoice")]
        [HttpGet("{interventionId}/links/invoice")]
        public async Task<IActionResult> GetRelatedInvoiceByIdAsync(int interventionId)
        {
            InvoiceDto invoiceDto;
            try
            {
                var invoice = await _invoiceManager.GetByInterventionIdAsync(interventionId);
                invoiceDto = _mapper.Map<InvoiceDto>(invoice);
            }
            catch (EntityNotFoundException)
            {
                invoiceDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = invoiceDto });
        }

        [HttpGet("{interventionId}/origin")]
        [HttpGet("{interventionId}/links/origin")]
        public async Task<IActionResult> GetRelatedOriginByIdAsync(int interventionId)
        {
            OrderDto orderDto;
            try
            {
                var order = await _orderManager.GetByInterventionIdAsync(interventionId);
                orderDto = _mapper.Map<OrderDto>(order);
            }
            catch (EntityNotFoundException)
            {
                orderDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = orderDto });
        }

        [HttpGet("{interventionId}/follow-up-request")]
        [HttpGet("{interventionId}/links/follow-up-request")]
        public async Task<IActionResult> GetRelatedFollUpRequestByIdAsync(int interventionId)
        {
            RequestDto requestDto;
            try
            {
                var request = await _requestManager.GetByInterventionIdAsync(interventionId);
                requestDto = _mapper.Map<RequestDto>(request);
            }
            catch (EntityNotFoundException)
            {
                requestDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = requestDto });
        }

        [HttpGet("{interventionId}/way-of-entry")]
        [HttpGet("{interventionId}/links/way-of-entry")]
        public async Task<IActionResult> GetRelatedWayOfEntryByIdAsync(int interventionId)
        {
            WayOfEntryDto wayOfEntryDto;
            try
            {
                var wayOfEntry = await _wayOfEntryManager.GetByInterventionIdAsync(interventionId);
                wayOfEntryDto = _mapper.Map<WayOfEntryDto>(wayOfEntry);
            }
            catch (EntityNotFoundException)
            {
                wayOfEntryDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = wayOfEntryDto });
        }


        [HttpGet("{interventionId}/employee")]
        [HttpGet("{interventionId}/links/employee")]
        public async Task<IActionResult> GetRelatedEmployeeByIdAsync(int interventionId)
        {
            EmployeeDto employeeDto;
            try
            {
                var employee = await _employeeManager.GetByInterventionIdAsync(interventionId);
                employeeDto = _mapper.Map<EmployeeDto>(employee);
            }
            catch (EntityNotFoundException)
            {
                employeeDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = employeeDto });
        }

        [HttpGet("{interventionId}/technicians")]
        [HttpGet("{interventionId}/links/technicians")]
        public async Task<IActionResult> GetRelatedTechniciansByIdAsync(int interventionId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedEmployees = await _employeeManager.GetAllByInterventionIdAsync(interventionId, querySet);

            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(pagedEmployees.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedEmployees.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedEmployees);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedEmployees);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = employeeDtos, Included = included });
        }
    }
}
