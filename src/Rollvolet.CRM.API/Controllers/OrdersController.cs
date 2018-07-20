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
using Rollvolet.CRM.APIContracts.DTO.Orders;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderManager _orderManager;
        private readonly IDepositManager _depositManager;
        private readonly IDepositInvoiceManager _depositInvoiceManager;
        private readonly IIncludedCollector _includedCollector;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public OrdersController(IOrderManager orderManager, IDepositManager depositManager, IDepositInvoiceManager depositInvoiceManager,
                                IIncludedCollector includedCollector, IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _orderManager = orderManager;
            _depositManager = depositManager;
            _depositInvoiceManager = depositInvoiceManager;
            _includedCollector = includedCollector;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
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
        public async Task<IActionResult> GetById(int id)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var order = await _orderManager.GetByIdAsync(id, querySet);

            var orderDto = _mapper.Map<OrderDto>(order, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(order, querySet.Include);
            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path, querySet);

            return Ok(new ResourceResponse() { Links = links, Data = orderDto, Included = included });
        }

        [HttpGet("{orderId}/deposits")]
        [HttpGet("{orderId}/links/deposits")]
        public async Task<IActionResult> GetRelatedDepositsByOrderId(int orderId)
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
        public async Task<IActionResult> GetRelatedDepositInvoicesByOrderId(int orderId)
        {
            var querySet = _jsonApiBuilder.BuildQuerySet(HttpContext.Request.Query);

            var pagedInvoices = await _depositInvoiceManager.GetAllByOrderIdAsync(orderId, querySet);

            var depositInvoiceDtos = _mapper.Map<IEnumerable<DepositInvoiceDto>>(pagedInvoices.Items, opt => opt.Items["include"] = querySet.Include);
            var included = _includedCollector.CollectIncluded(pagedInvoices.Items, querySet.Include);
            var links = _jsonApiBuilder.BuildCollectionLinks(HttpContext.Request.Path, querySet, pagedInvoices);
            var meta = _jsonApiBuilder.BuildCollectionMetadata(pagedInvoices);

            return Ok(new ResourceResponse() { Meta = meta, Links = links, Data = depositInvoiceDtos, Included = included });
        }
    }
}