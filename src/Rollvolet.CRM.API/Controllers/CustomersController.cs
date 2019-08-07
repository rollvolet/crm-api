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
using Rollvolet.CRM.APIContracts.DTO.DepositInvoices;
using Rollvolet.CRM.APIContracts.DTO.Invoices;
using Rollvolet.CRM.APIContracts.DTO.Offers;
using Rollvolet.CRM.APIContracts.DTO.Orders;
using Rollvolet.CRM.APIContracts.DTO.Requests;
using Rollvolet.CRM.APIContracts.DTO.Telephones;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerManager _customerManager;
        private readonly IContactManager _contactManager;
        private readonly IBuildingManager _buildingManager;
        private readonly ITelephoneManager _telephoneManager;
        private readonly IRequestManager _requestManager;
        private readonly IOfferManager _offerManager;
        private readonly IOrderManager _orderManager;
        private readonly IDepositInvoiceManager _depositInvoiceManager;
        private readonly IInvoiceManager _invoiceManager;
        private readonly ITagManager _tagManager;
        private readonly ICountryManager _countryManager;
        private readonly ILanguageManager _languageManager;
        private readonly IHonorificPrefixManager _honorificPrefixManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public CustomersController(ICustomerManager customerManager, IContactManager contactManager, IBuildingManager buildingManager,
                                    ITelephoneManager telephoneManager, IRequestManager requestManager, IOfferManager offerManager,
                                    IOrderManager orderManager, IDepositInvoiceManager depositInvoiceManager, IInvoiceManager invoiceManager,
                                    ITagManager tagManager, ICountryManager countryManager, ILanguageManager languageManager,
                                    IHonorificPrefixManager honorificPrefixManager, IIncludedCollector includedCollector,
                                    IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _customerManager = customerManager;
            _contactManager = contactManager;
            _buildingManager = buildingManager;
            _telephoneManager = telephoneManager;
            _requestManager = requestManager;
            _offerManager = offerManager;
            _orderManager = orderManager;
            _depositInvoiceManager = depositInvoiceManager;
            _invoiceManager = invoiceManager;
            _tagManager = tagManager;
            _countryManager = countryManager;
            _languageManager = languageManager;
            _honorificPrefixManager = honorificPrefixManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedCustomers = await _customerManager.GetAllAsync(querySet);

            var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(pagedCustomers.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedCustomers.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedCustomers);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedCustomers);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = customerDtos, Included = included });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var customer = await _customerManager.GetByIdAsync(id, querySet);

            var customerDto = _mapper.Map<CustomerDto>(customer, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(customer, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = customerDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<CustomerRequestDto> resource)
        {
            if (resource.Data.Type != "customers") return StatusCode(409);

            var customer = _mapper.Map<Customer>(resource.Data);

            customer = await _customerManager.CreateAsync(customer);
            var customerDto = _mapper.Map<CustomerDto>(customer);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, customerDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = customerDto });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] ResourceRequest<CustomerRequestDto> resource)
        {
            if (resource.Data.Type != "customers" || resource.Data.Id != id) return StatusCode(409);

            var customer = _mapper.Map<Customer>(resource.Data);

            customer = await _customerManager.UpdateAsync(customer);

            var customerDto = _mapper.Map<CustomerDto>(customer);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);

            return Ok(new ResourceResponse() { Links = links, Data = customerDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _customerManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("{customerId}/contacts")]
        [HttpGet("{customerId}/links/contacts")]
        public async Task<IActionResult> GetRelatedContactsByCustomerIdAsync(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedContacts = await _contactManager.GetAllByCustomerIdAsync(customerId, querySet);

            var contactDtos = _mapper.Map<IEnumerable<ContactDto>>(pagedContacts.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedContacts.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedContacts);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedContacts);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = contactDtos, Included = included });
        }

        [HttpGet("{customerId}/buildings")]
        [HttpGet("{customerId}/links/buildings")]
        public async Task<IActionResult> GetRelatedBuildingsByIdAsync(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedBuildings = await _buildingManager.GetAllByCustomerIdAsync(customerId, querySet);

            var buildingDtos = _mapper.Map<IEnumerable<BuildingDto>>(pagedBuildings.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedBuildings.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedBuildings);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedBuildings);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = buildingDtos, Included = included });
        }

        [HttpGet("{customerId}/telephones")]
        [HttpGet("{customerId}/links/telephones")]
        public async Task<IActionResult> GetRelatedTelephonesByIdAsync(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);
            querySet.Include.Fields = new string[] {"country", "telephone-type"};

            var pagedTelephones = await _telephoneManager.GetAllByCustomerIdAsync(customerId, querySet);

            var telephoneDtos = _mapper.Map<IEnumerable<TelephoneDto>>(pagedTelephones.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedTelephones.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedTelephones);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedTelephones);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = telephoneDtos, Included = included });
        }

        [HttpGet("{customerId}/requests")]
        [HttpGet("{customerId}/links/requests")]
        public async Task<IActionResult> GetRelatedRequestsByIdAsync(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedRequests = await _requestManager.GetAllByCustomerIdAsync(customerId, querySet);

            var requestDtos = _mapper.Map<IEnumerable<RequestDto>>(pagedRequests.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedRequests.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedRequests);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedRequests);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = requestDtos, Included = included });
        }

        [HttpGet("{customerId}/offers")]
        [HttpGet("{customerId}/links/offers")]
        public async Task<IActionResult> GetRelatedOffersByIdAsync(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedOffers = await _offerManager.GetAllByCustomerIdAsync(customerId, querySet);

            var offerDtos = _mapper.Map<IEnumerable<OfferDto>>(pagedOffers.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedOffers.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedOffers);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedOffers);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = offerDtos, Included = included });
        }

        [HttpGet("{customerId}/orders")]
        [HttpGet("{customerId}/links/orders")]
        public async Task<IActionResult> GetRelatedOrdersByIdAsync(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedOrders = await _orderManager.GetAllByCustomerIdAsync(customerId, querySet);

            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(pagedOrders.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedOrders.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedOrders);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedOrders);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = orderDtos, Included = included });
        }

        [HttpGet("{customerId}/deposit-invoices")]
        [HttpGet("{customerId}/links/deposit-invoices")]
        public async Task<IActionResult> GetRelatedDepositInvoicesByIdAsync(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedInvoices = await _depositInvoiceManager.GetAllByCustomerIdAsync(customerId, querySet);

            var invoiceDtos = _mapper.Map<IEnumerable<DepositInvoiceDto>>(pagedInvoices.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedInvoices.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedInvoices);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedInvoices);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = invoiceDtos, Included = included });
        }

        [HttpGet("{customerId}/invoices")]
        [HttpGet("{customerId}/links/invoices")]
        public async Task<IActionResult> GetRelatedInvoicesByIdAsync(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedInvoices = await _invoiceManager.GetAllByCustomerIdAsync(customerId, querySet);

            var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(pagedInvoices.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedInvoices.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedInvoices);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedInvoices);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = invoiceDtos, Included = included });
        }

        [HttpGet("{customerId}/tags")]
        [HttpGet("{customerId}/links/tags")]
        public async Task<IActionResult> GetRelatedTagsByIdAsync(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedTags = await _tagManager.GetAllByCustomerIdAsync(customerId, querySet);

            var tagDtos = _mapper.Map<IEnumerable<TagDto>>(pagedTags.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedTags.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedTags);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedTags);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = tagDtos, Included = included });
        }

        [HttpGet("{customerId}/country")]
        [HttpGet("{customerId}/links/country")]
        public async Task<IActionResult> GetRelatedCountryByIdAsync(int customerId)
        {
            CountryDto countryDto;
            try
            {
                var country = await _countryManager.GetByCustomerIdAsync(customerId);
                countryDto = _mapper.Map<CountryDto>(country);
            }
            catch (EntityNotFoundException)
            {
                countryDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = countryDto });
        }

        [HttpGet("{customerId}/language")]
        [HttpGet("{customerId}/links/language")]
        public async Task<IActionResult> GetRelatedLanguageByIdAsync(int customerId)
        {
            LanguageDto languageDto;
            try
            {
                var language = await _languageManager.GetByCustomerIdAsync(customerId);
                languageDto = _mapper.Map<LanguageDto>(language);
            }
            catch (EntityNotFoundException)
            {
                languageDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = languageDto });
        }

        [HttpGet("{customerId}/honorific-prefix")]
        [HttpGet("{customerId}/links/honorific-prefix")]
        public async Task<IActionResult> GetRelatedHonorificPrefixByIdAsync(int customerId)
        {
            HonorificPrefixDto honorificPrefixDto;
            try
            {
                var honorificPrefix = await _honorificPrefixManager.GetByCustomerIdAsync(customerId);
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