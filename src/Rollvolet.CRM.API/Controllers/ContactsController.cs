using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.DTO.Contacts;
using Rollvolet.CRM.APIContracts.DTO.Invoices;
using Rollvolet.CRM.APIContracts.DTO.Requests;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactManager _contactManager;

        private readonly ICountryManager _countryManager;
        private readonly ILanguageManager _languageManager;
        private readonly IHonorificPrefixManager _honorificPrefixManager;
        private readonly IRequestManager _requestManager;
        private readonly IInvoiceManager _invoiceManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public ContactsController(IContactManager contactManager, ICountryManager countryManager,
                                    ILanguageManager languageManager, IHonorificPrefixManager honorificPrefixManager,
                                    IRequestManager requestManager, IInvoiceManager invoiceManager,
                                    IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _contactManager = contactManager;
            _countryManager = countryManager;
            _languageManager = languageManager;
            _honorificPrefixManager = honorificPrefixManager;
            _requestManager = requestManager;
            _invoiceManager = invoiceManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedContacts = await _contactManager.GetAllAsync(querySet);

            var contactDtos = _mapper.Map<IEnumerable<ContactDto>>(pagedContacts.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedContacts.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedContacts);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedContacts);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = contactDtos, Included = included });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var contact = await _contactManager.GetByIdAsync(id, querySet);

            var contactDto = _mapper.Map<ContactDto>(contact, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(contact, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = contactDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<ContactRequestDto> resource)
        {
            if (resource.Data.Type != "contacts") return StatusCode(409);

            var contact = _mapper.Map<Contact>(resource.Data);

            contact = await _contactManager.CreateAsync(contact);

            var contactDto = _mapper.Map<ContactDto>(contact);
            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, contactDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = contactDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ResourceRequest<ContactRequestDto> resource)
        {
            if (resource.Data.Type != "contacts" || resource.Data.Id != id) return StatusCode(409);

            var contact = _mapper.Map<Contact>(resource.Data);

            contact = await _contactManager.UpdateAsync(contact);

            var contactDto = _mapper.Map<ContactDto>(contact);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = contactDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _contactManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("{contactId}/requests")]
        [HttpGet("{contactId}/links/requests")]
        public async Task<IActionResult> GetRelatedRequestsByIdAsync(int contactId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedRequests = await _requestManager.GetAllByContactIdAsync(contactId, querySet);

            var requestDtos = _mapper.Map<IEnumerable<RequestDto>>(pagedRequests.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedRequests.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedRequests);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedRequests);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = requestDtos, Included = included });
        }

        [HttpGet("{contactId}/invoices")]
        [HttpGet("{contactId}/links/invoices")]
        public async Task<IActionResult> GetRelatedInvoicesByIdAsync(int contactId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedInvoices = await _invoiceManager.GetAllByContactIdAsync(contactId, querySet);

            var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(pagedInvoices.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedInvoices.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedInvoices);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedInvoices);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = invoiceDtos, Included = included });
        }

        [HttpGet("{contactId}/country")]
        [HttpGet("{contactId}/links/country")]
        public async Task<IActionResult> GetRelatedCountryByIdAsync(int contactId)
        {
            CountryDto countryDto;
            try
            {
                var country = await _countryManager.GetByContactIdAsync(contactId);
                countryDto = _mapper.Map<CountryDto>(country);
            }
            catch (EntityNotFoundException)
            {
                countryDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = countryDto });
        }

        [HttpGet("{contactId}/language")]
        [HttpGet("{contactId}/links/language")]
        public async Task<IActionResult> GetRelatedLanguageByIdAsync(int contactId)
        {
            LanguageDto languageDto;
            try
            {
                var language = await _languageManager.GetByContactIdAsync(contactId);
                languageDto = _mapper.Map<LanguageDto>(language);
            }
            catch (EntityNotFoundException)
            {
                languageDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = languageDto });
        }

        [HttpGet("{contactId}/honorific-prefix")]
        [HttpGet("{contactId}/links/honorific-prefix")]
        public async Task<IActionResult> GetRelatedHonorificPrefixByIdAsync(int contactId)
        {
            HonorificPrefixDto honorificPrefixDto;
            try
            {
                var honorificPrefix = await _honorificPrefixManager.GetByContactIdAsync(contactId);
                honorificPrefixDto = _mapper.Map<HonorificPrefixDto>(honorificPrefix);
            }
            catch (EntityNotFoundException)
            {
                honorificPrefixDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = honorificPrefixDto });
        }
    }
}