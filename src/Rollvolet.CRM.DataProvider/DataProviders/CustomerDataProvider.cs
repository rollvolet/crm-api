using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Extensions;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProviders
{
    public class CustomerDataProvider : CustomerRecordDataProvider, ICustomerDataProvider
    {
        public CustomerDataProvider(CrmContext context, IMapper mapper, ISequenceDataProvider sequenceDataProvider,
                                    ILogger<CustomerDataProvider> logger) : base(context, mapper, sequenceDataProvider, logger)
        {
        }

        public async Task<Paged<Customer>> GetAllAsync(QuerySet query)
        {
            var source = BaseQuery()
                            .Include(query)
                            .Sort(query)
                            .Filter(query);

            var customers = await source.ForPage(query).ToListAsync();

            var mappedCustomers = _mapper.Map<IEnumerable<Customer>>(customers);

            var count = await source.CountAsync();

            return new Paged<Customer>() {
                Items = mappedCustomers,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Customer> GetByNumberAsync(int number, QuerySet query = null)
        {
            var customer = await FindByNumberAsync(number, query);

            if (customer == null)
            {
                _logger.LogError($"No customer found with number {number}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Customer>(customer);
        }

        public async Task<Customer> GetByRequestIdAsync(int requestId)
        {
            var customerNumber = await _context.Requests.Where(r => r.Id == requestId).Select(r => r.CustomerId).FirstOrDefaultAsync();

            if (customerNumber == null)
            {
                _logger.LogError($"No customer found for request id {requestId}");
                throw new EntityNotFoundException();
            }

            var customer = await FindByNumberAsync((int) customerNumber);

            return _mapper.Map<Customer>(customer);
        }

        public async Task<Customer> GetByInterventionIdAsync(int interventionId)
        {
            var customerNumber = await _context.Interventions.Where(r => r.Id == interventionId).Select(r => r.CustomerId).FirstOrDefaultAsync();

            if (customerNumber == null)
            {
                _logger.LogError($"No customer found for intervention id {interventionId}");
                throw new EntityNotFoundException();
            }

            var customer = await FindByNumberAsync((int) customerNumber);

            return _mapper.Map<Customer>(customer);
        }

        public async Task<Customer> GetByOfferIdAsync(int offerId)
        {
            var customerNumber = await _context.Offers.Where(r => r.Id == offerId).Select(r => r.CustomerId).FirstOrDefaultAsync();

            if (customerNumber == null)
            {
                _logger.LogError($"No customer found for offer id {offerId}");
                throw new EntityNotFoundException();
            }

            var customer = await FindByNumberAsync((int) customerNumber);

            return _mapper.Map<Customer>(customer);
        }

        public async Task<Customer> GetByOrderIdAsync(int orderId)
        {
            var customerNumber = await _context.Orders.Where(r => r.Id == orderId).Select(r => r.CustomerId).FirstOrDefaultAsync();

            if (customerNumber == null)
            {
                _logger.LogError($"No customer found for order id {orderId}");
                throw new EntityNotFoundException();
            }

            var customer = await FindByNumberAsync((int) customerNumber);

            return _mapper.Map<Customer>(customer);
        }

        public async Task<Customer> GetByInvoiceIdAsync(int invoiceId)
        {
            var customerNumber = await _context.Invoices.Where(r => r.Id == invoiceId).Select(r => r.CustomerId).FirstOrDefaultAsync();

            if (customerNumber == null)
            {
                _logger.LogError($"No customer found for invoice id {invoiceId}");
                throw new EntityNotFoundException();
            }

            var customer = await FindByNumberAsync((int) customerNumber);

            return _mapper.Map<Customer>(customer);
        }

        public async Task<Customer> GetByDepositInvoiceIdAsync(int depositInvoiceId)
        {
            return await GetByInvoiceIdAsync(depositInvoiceId);
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            var customerRecord = _mapper.Map<DataProvider.Models.Customer>(customer);

            customerRecord.Number = await _sequenceDataProvider.GetNextCustomerNumberAsync();
            customerRecord.Created = DateTimeOffset.UtcNow.UtcDateTime;
            customerRecord.SearchName = DataProvider.Models.CustomerRecord.CalculateSearchName(customer.Name);
            // customer name is already uppercased by the frontend

            await HydratePostalCodeAsync(customer, customerRecord);

            if (customer.Memo != null)
            {
                var memo = new DataProvider.Models.Memo() { Text = customer.Memo };
                customerRecord.Memo = memo;
                _context.Memos.Add(memo);
            }

            _context.Customers.Add(customerRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Customer>(customerRecord);
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            var customerRecord = await FindByNumberAsync(customer.Id);
            var memoRecord = customerRecord.Memo;
            _mapper.Map(customer, customerRecord);
            customerRecord.SearchName = DataProvider.Models.CustomerRecord.CalculateSearchName(customer.Name);
            // customer name is already uppercased by the frontend

            await HydratePostalCodeAsync(customer, customerRecord);
            ReplaceEmptyStringWithNull(customer, customerRecord);

            if (customer.Memo != null)
            {
                if (memoRecord != null)  // update existing memo
                {
                    memoRecord.Text = customer.Memo;
                }
                else  // create new memo
                {
                    var memo = new DataProvider.Models.Memo() { Text = customer.Memo };
                    customerRecord.Memo = memo;
                    _context.Memos.Add(memo);
                }
            }
            else if (memoRecord != null)  // delete old memo
                _context.Memos.Remove(memoRecord);

            _context.Customers.Update(customerRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Customer>(customerRecord);
        }

        public async Task DeleteByNumberAsync(int number)
        {
            var customer = await FindByNumberAsync(number);

            if (customer != null)
            {
                if (customer.Memo != null)
                {
                    _context.Memos.Remove(customer.Memo);
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
           }
        }

        private IQueryable<DataProvider.Models.Customer> BaseQuery()
        {
            return _context.Customers
                        .Include(e => e.Memo);
        }

        private async Task<DataProvider.Models.Customer> FindByNumberAsync(int number, QuerySet query = null)
        {
            return await FindWhereAsync(c => c.Number == number, query);
        }

        private async Task<DataProvider.Models.Customer> FindByDataIdAsync(int id, QuerySet query = null)
        {
            return await FindWhereAsync(c => c.DataId == id, query);
        }

        private async Task<DataProvider.Models.Customer> FindWhereAsync(Expression<Func<DataProvider.Models.Customer, bool>> where,
                                                                        QuerySet query = null)
        {
            var source = BaseQuery().Where(where);

            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }
    }
}