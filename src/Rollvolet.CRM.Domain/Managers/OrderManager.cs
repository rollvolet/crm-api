using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class OrderManager : IOrderManager
    {
        private readonly IOrderDataProvider _orderDataProvider;
        private readonly ILogger _logger;

        public OrderManager(IOrderDataProvider orderDataProvider, ILogger<OrderManager> logger)
        {
            _orderDataProvider = orderDataProvider;
            _logger = logger;
        }
        
        public async Task<Paged<Order>> GetAllAsync(QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "order-date";
            }

            return await _orderDataProvider.GetAllAsync(query);
        }

        public async Task<Order> GetByIdAsync(int id, QuerySet query)
        {
            return await _orderDataProvider.GetByIdAsync(id, query);
        }

        public async Task<Paged<Order>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "order-date";
            }

            return await _orderDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }
    }
}