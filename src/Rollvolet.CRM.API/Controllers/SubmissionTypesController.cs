using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("submission-types")]
    [Authorize]
    public class SubmissionTypesController : Controller
    {
        private readonly ISubmissionTypeManager _submissionTypeManager;
        private readonly IMapper _mapper;

        public SubmissionTypesController(ISubmissionTypeManager submissionTypeManager, IMapper mapper)
        {
            _submissionTypeManager = submissionTypeManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var submissionTypes = await _submissionTypeManager.GetAllAsync();

            var submissionTypeDtos = _mapper.Map<IEnumerable<SubmissionTypeDto>>(submissionTypes);
            var links = new CollectionLinks() { Self = HttpContext.Request.Path };

            return Ok(new ResourceResponse() { Links = links, Data = submissionTypeDtos });
        }
    }
}