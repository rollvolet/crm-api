using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.DataProvider.Extensions;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Exceptions;
using System.Linq.Expressions;
using System;

namespace Rollvolet.CRM.DataProviders
{
    public class OrderDataProvider : IOrderDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public OrderDataProvider(CrmContext context, IMapper mapper, ILogger<OrderDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Paged<Order>> GetAllAsync(QuerySet query)
        {
            var source = _context.Orders
                            .Include(query)
                            .Sort(query)
                            .Filter(query, _context);

            var orders = source.ForPage(query).AsEnumerable();

            var mappedOrders = _mapper.Map<IEnumerable<Order>>(orders);

            var count = await source.CountAsync();

            return new Paged<Order>() {
                Items = mappedOrders,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Order> GetByIdAsync(int id, QuerySet query = null)
        {
            var order = await FindByIdAsync(id, query);

            if (order == null)
            {
                _logger.LogError($"No order found with id {id}");
                throw new EntityNotFoundException();
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

            var orders = source.ForPage(query).AsEnumerable();

            var mappedOrders = _mapper.Map<IEnumerable<Order>>(orders);

            var count = await source.CountAsync();

            return new Paged<Order>() {
                Items = mappedOrders,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Order> GetByInterventionIdAsync(int interventionId, QuerySet query = null)
        {
            var intervention = await _context.Interventions.Where(i => i.Id == interventionId).FirstOrDefaultAsync();
            var orderId = intervention.OriginId;

            if (orderId == null)
            {
                _logger.LogError($"No order found for intervention-id {interventionId}");
                throw new EntityNotFoundException();
            }

            return await GetByIdAsync((int) orderId, query);
        }

        public async Task<Order> GetByOfferIdAsync(int offerId, QuerySet query = null)
        {
            var order = await FindByIdAsync(offerId, query); // order has the same id as the attached offer

            if (order == null)
            {
                _logger.LogError($"No order found for offer-id {offerId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Order>(order);
        }

        public async Task<Order> GetByInvoiceIdAsync(int invoiceId, QuerySet query = null)
        {
            var invoice = await _context.Invoices.Where(i => i.Id == invoiceId).FirstOrDefaultAsync();
            var orderId = invoice.OrderId;

            if (orderId == null)
            {
                _logger.LogError($"No order found for invoice-id {invoiceId}");
                throw new EntityNotFoundException();
            }

            return await GetByIdAsync((int) orderId, query);
        }

        public async Task<Order> GetByInvoicelineIdAsync(int invoicelineId)
        {
            var order = await _context.Invoicelines.Where(o => o.Id == invoicelineId).Select(o => o.Order).FirstOrDefaultAsync();

            if (order == null)
            {
                _logger.LogError($"No order found for invoiceline-id {invoicelineId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Order>(order);
        }

        public async Task<Order> GetByDepositInvoiceIdAsync(int invoiceId, QuerySet query = null)
        {
            return await GetByInvoiceIdAsync(invoiceId, query);
        }

        public async Task<Order> CreateAsync(Order order)
        {
            // Order has already been created by EF Core on creation of the offer since they share the same underlying SQL table
            var existingOrderRecord = await _context.Orders.Where(o => o.Id == order.Offer.Id).IgnoreQueryFilters().FirstOrDefaultAsync();

            if (existingOrderRecord == null) {
                var message = $"Expected order {order.Id} to exist already in EF Core, but none was found.";
                _logger.LogError(message);
                throw new CodedException("InvalidState", "Unable to create order", message);
            }

            order.Id = existingOrderRecord.Id; // Set existing id on the new incoming order. Id of the incoming order is null.

            // Make it a valid order
            existingOrderRecord.IsOrdered = true;
            existingOrderRecord.Currency = "EUR";

            _mapper.Map(order, existingOrderRecord);

            _context.Orders.Update(existingOrderRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Order>(existingOrderRecord);
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            var orderRecord = await FindByIdAsync(order.Id);
            _mapper.Map(order, orderRecord);

            _context.Orders.Update(orderRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Order>(orderRecord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var orderRecord = await FindByIdAsync(id);

            if (orderRecord != null)
            {
                // Don't remove record, but only reset order-specific attributes because order and offer share the same underlying SQL table
                orderRecord.OrderDate = null;
                orderRecord.DepositRequired = false;
                orderRecord.HasProductionTicket = false;
                orderRecord.MustBeInstalled = false;
                orderRecord.MustBeDelivered = false;
                orderRecord.IsReady = false;
                orderRecord.ExpectedDate = null;
                orderRecord.RequiredDate = null;
                orderRecord.ScheduledHours = null;
                orderRecord.ScheduledNbOfPersons = null;
                orderRecord.Canceled = false;
                orderRecord.CancellationReason = null;
                orderRecord.Currency = null;
                orderRecord.IsOrdered = false;
                orderRecord.PlanningDate = null;
                orderRecord.PlanningMsObjectId = null;

                _context.Orders.Update(orderRecord);
                await _context.SaveChangesAsync();
           }
        }

        private async Task<DataProvider.Models.Order> FindByIdAsync(int id, QuerySet query = null)
        {
            return await FindWhereAsync(c => c.Id == id, query);
        }

        private async Task<DataProvider.Models.Order> FindWhereAsync(Expression<Func<DataProvider.Models.Order, bool>> where, QuerySet query = null)
        {
            var source = _context.Orders.Where(where);

            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }
    }
}