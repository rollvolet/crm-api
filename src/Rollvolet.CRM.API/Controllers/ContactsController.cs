using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.API.Builders;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.APIContracts.DTO.Contacts;
using Rollvolet.CRM.APIContracts.DTO.Telephones;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class ContactsController : Controller
    {
        private readonly IContactManager _contactManager;
        private readonly ITelephoneManager _telephoneManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public ContactsController(IContactManager contactManager, ITelephoneManager telephoneManager, IIncludedCollector includedCollector,
                                    IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _contactManager = contactManager;
            _telephoneManager = telephoneManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var contact = await _contactManager.GetByIdAsync(id, querySet);

            var contactDto = _mapper.Map<ContactDto>(contact, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(contact, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = contactDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResourceRequest<ContactRequestDto> resource)
        {
            // TODO return 409 if type is not correct
            var contact = _mapper.Map<Contact>(resource.Data);

            contact = await _contactManager.CreateAsync(contact);

            var contactDto = _mapper.Map<ContactDto>(contact);
            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, contactDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = contactDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update([FromBody] ResourceRequest<ContactRequestDto> resource)
        {
            // TODO return 409 if id and type are not correct
            var contact = _mapper.Map<Contact>(resource.Data);

            contact = await _contactManager.UpdateAsync(contact);

            var contactDto = _mapper.Map<ContactDto>(contact);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = contactDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _contactManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("{contactId}/telephones")]
        [HttpGet("{contactId}/links/telephones")]
        public async Task<IActionResult> GetRelatedTelephonesById(int contactId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);
            querySet.Include.Fields = new string[] {"country", "telephone-type"};

            var pagedTelephones = await _telephoneManager.GetAllByContactIdAsync(contactId, querySet);

            var telephoneDtos = _mapper.Map<IEnumerable<TelephoneDto>>(pagedTelephones.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedTelephones.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedTelephones);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedTelephones);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = telephoneDtos, Included = included });
        }

    }
}