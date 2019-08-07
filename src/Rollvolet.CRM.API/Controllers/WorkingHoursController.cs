using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.DTO.Invoices;
using Rollvolet.CRM.APIContracts.DTO.WorkingHours;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("working-hours")]
    [Authorize]
    public class WorkingHoursController : ControllerBase
    {
        private readonly IWorkingHourManager _workingHourManager;
        private readonly IEmployeeManager _employeeManager;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public WorkingHoursController(IWorkingHourManager workingHourManager, IEmployeeManager employeeManager, IInvoiceManager invoiceManager,
                                    IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _workingHourManager = workingHourManager;
            _employeeManager = employeeManager;
            _invoiceManager = invoiceManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsyncAsync([FromBody] ResourceRequest<WorkingHourRequestDto> resource)
        {
            if (resource.Data.Type != "working-hours") return StatusCode(409);

            var workingHour = _mapper.Map<WorkingHour>(resource.Data);

            workingHour = await _workingHourManager.CreateAsync(workingHour);
            var workingHourDto = _mapper.Map<WorkingHourDto>(workingHour);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, workingHourDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = workingHourDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsyncAsync(string id, [FromBody] ResourceRequest<WorkingHourRequestDto> resource)
        {
            if (resource.Data.Type != "working-hours" || resource.Data.Id != id) return StatusCode(409);

            var workingHour = _mapper.Map<WorkingHour>(resource.Data);

            workingHour = await _workingHourManager.UpdateAsync(workingHour);

            var workingHourDto = _mapper.Map<WorkingHourDto>(workingHour);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = workingHourDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _workingHourManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("{workingHourId}/employee")]
        [HttpGet("{workingHourId}/links/employee")]
        public async Task<IActionResult> GetRelatedEmployeeByIdAsync(int workingHourId)
        {
            EmployeeDto employeeDto;
            try
            {
                var employee = await _employeeManager.GetByWorkingHourIdAsync(workingHourId);
                employeeDto = _mapper.Map<EmployeeDto>(employee);
            }
            catch (EntityNotFoundException)
            {
                employeeDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = employeeDto });
        }

        [HttpGet("{workingHourId}/invoice")]
        [HttpGet("{workingHourId}/links/invoice")]
        public async Task<IActionResult> GetRelatedInvoiceByIdAsync(int workingHourId)
        {
            InvoiceDto invoiceDto;
            try
            {
                var invoice = await _invoiceManager.GetByWorkingHourIdAsync(workingHourId);
                invoiceDto = _mapper.Map<InvoiceDto>(invoice);
            }
            catch (EntityNotFoundException)
            {
                invoiceDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = invoiceDto });
        }
    }
}