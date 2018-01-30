using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Narato.ResponseMiddleware.Models.Exceptions;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.DataProvider.Extensions;
using Microsoft.Extensions.Logging;
using LinqKit;

namespace Rollvolet.CRM.DataProviders
{   
    public class OrderDataProvider : CaseRelatedDataProvider<DataProvider.Models.Order>, IOrderDataProvider
    {

        public OrderDataProvider(CrmContext context, IMapper mapper, ILogger<OrderDataProvider> logger) : base(context, mapper, logger)
        {

        }

        public async Task<Paged<Order>> GetAllAsync(QuerySet query)
        {
            var source = _context.Orders
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

            // EF Core doesn't support relationships with a derived type so we have to embed the related resource manually
            var orders = QueryListWithManualInclude(source, query);

            var mappedOrders = _mapper.Map<IEnumerable<Order>>(orders);

            var count = await source.CountAsync();

            return new Paged<Order>() {
                Items = mappedOrders,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Order> GetByIdAsync(int id, QuerySet query)
        {
            var source = _context.Orders
                            .Where(c => c.Id == id)
                            .Include(query);

            // EF Core doesn't support relationships with a derived type so we have to embed the related resource manually
            var order = await QueryWithManualIncludeAsync(source, query);

            if (order == null)
            {
                // TODO implement and handle exceptions according to jsonapi
                _logger.LogError($"No order found with id {id}");
                throw new EntityNotFoundException("ENF", $"Order with id {id} not found");
            }

            return _mapper.Map<Order>(order);
        }

        public async Task<Paged<Order>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            var source = _context.Orders
                            .Where(o => o.CustomerId == customerId)
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

            var orders = QueryListWithManualInclude(source, query);

            var mappedOrders = _mapper.Map<IEnumerable<Order>>(orders);

            var count = await source.CountAsync();

            return new Paged<Order>() {
                Items = mappedOrders,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };            
        }     
    }
}