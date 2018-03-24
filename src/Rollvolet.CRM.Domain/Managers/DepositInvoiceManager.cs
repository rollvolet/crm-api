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
    public class DepositInvoiceManager : IDepositInvoiceManager
    {
        private readonly IDepositInvoiceDataProvider _depositInvoiceDataProvider;
        private readonly ILogger _logger;

        public DepositInvoiceManager(IDepositInvoiceDataProvider depositInvoiceDataProvider, ILogger<DepositInvoiceManager> logger)
        {
            _depositInvoiceDataProvider = depositInvoiceDataProvider;
            _logger = logger;
        }
        
        public async Task<Paged<DepositInvoice>> GetAllAsync(QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "number";
            }

            return await _depositInvoiceDataProvider.GetAllAsync(query);
        }

        public async Task<DepositInvoice> GetByIdAsync(int id, QuerySet query)
        {
            return await _depositInvoiceDataProvider.GetByIdAsync(id, query);
        }

        public async Task<Paged<DepositInvoice>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "number";
            }

            return await _depositInvoiceDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }

        public async Task<Paged<DepositInvoice>> GetAllByOrderIdAsync(int orderId, QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "number";
            }

            return await _depositInvoiceDataProvider.GetAllByOrderIdAsync(orderId, query);
        }
    }
}