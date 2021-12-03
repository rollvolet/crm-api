
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.APIContracts.DTO.Reports;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Business.Managers.Interfaces;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportManager _reportManager;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public ReportsController(IReportManager reportManager, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _reportManager = reportManager;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetMonthlySalesReport([FromQuery] int? fromYear, [FromQuery] int? toYear)
        {
            if (toYear == null)
                toYear = DateTimeOffset.UtcNow.UtcDateTime.Year;
            if (fromYear == null)
                fromYear = toYear - 4;

            var entries = await _reportManager.GetMonthlySalesReport((int) fromYear, (int) toYear);
            var entryDtos = _mapper.Map<IEnumerable<MonthlySalesEntryDto>>(entries);

            return Ok(new ResourceResponse() { Data = entryDtos });
        }

        [HttpGet("outstanding-jobs")]
        public async Task<IActionResult> GetOutstandingJobs()
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedOutstandingJobs = await _reportManager.GetOutstandingJobs(querySet);

            var outstandingJobDtos = _mapper.Map<IEnumerable<OutstandingJobDto>>(pagedOutstandingJobs.Items);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedOutstandingJobs);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedOutstandingJobs);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = outstandingJobDtos });
        }

        [HttpGet("outstanding-job-report")]
        public async Task<IActionResult> GetOutstandingJobReport()
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var report = await _reportManager.GetOutstandingJobReport(querySet);
            var reportDto = _mapper.Map<OutstandingJobReportDto>(report);

            return Ok(new ResourceResponse() { Data = reportDto });
        }
    }
}