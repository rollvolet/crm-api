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
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.DTO.Customers;
using Rollvolet.CRM.APIContracts.DTO.Telephones;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class CustomersController : Controller
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
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public CustomersController(ICustomerManager customerManager, IContactManager contactManager, IBuildingManager buildingManager, 
                                    ITelephoneManager telephoneManager, IRequestManager requestManager, IOfferManager offerManager, 
                                    IOrderManager orderManager, IDepositInvoiceManager depositInvoiceManager, IInvoiceManager invoiceManager, 
                                    ITagManager tagManager, IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
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
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
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
        public async Task<IActionResult> GetById(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var customer = await _customerManager.GetByIdAsync(id, querySet);

            var customerDto = _mapper.Map<CustomerDto>(customer, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(customer, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = customerDto, Included = included });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResourceRequest<CustomerRequestDto> resource)
        {
            var customer = _mapper.Map<Customer>(resource.Data);

            customer = await _customerManager.CreateAsync(customer);
            var customerDto = _mapper.Map<CustomerDto>(customer);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, customerDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = customerDto });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _customerManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("{customerId}/contacts")]
        [HttpGet("{customerId}/links/contacts")]
        public async Task<IActionResult> GetRelatedContactsByCustomerId(int customerId)
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
        public async Task<IActionResult> GetRelatedBuildingsById(int customerId)
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
        public async Task<IActionResult> GetRelatedTelephonesById(int customerId)
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
        public async Task<IActionResult> GetRelatedRequestsById(int customerId)
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
        public async Task<IActionResult> GetRelatedOffersById(int customerId)
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
        public async Task<IActionResult> GetRelatedOrdersById(int customerId)
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
        public async Task<IActionResult> GetRelatedDepositInvoicesById(int customerId)
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
        public async Task<IActionResult> GetRelatedInvoicesById(int customerId)
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
        public async Task<IActionResult> GetRelatedTagsById(int customerId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedTags = await _tagManager.GetAllByCustomerIdAsync(customerId, querySet);

            var tagDtos = _mapper.Map<IEnumerable<TagDto>>(pagedTags.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedTags.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedTags);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedTags);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = tagDtos, Included = included });
        }
    }
} 