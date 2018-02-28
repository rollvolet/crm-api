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
    public class InvoiceManager : IInvoiceManager
    {
        private readonly IInvoiceDataProvider _invoiceDataProvider;
        private readonly ILogger _logger;

        public InvoiceManager(IInvoiceDataProvider invoiceDataProvider, ILogger<InvoiceManager> logger)
        {
            _invoiceDataProvider = invoiceDataProvider;
            _logger = logger;
        }
        
        public async Task<Paged<Invoice>> GetAllAsync(QuerySet query)
        {
            if (query.Sort.Field == null) {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "number";
            }

            return await _invoiceDataProvider.GetAllAsync(query);
        }

        public async Task<Invoice> GetByIdAsync(int id, QuerySet query)
        {
            return await _invoiceDataProvider.GetByIdAsync(id, query);
        }

        public async Task<Paged<Invoice>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            if (query.Sort.Field == null) {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "number";
            }

            return await _invoiceDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }
    }
}