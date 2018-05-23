using System;
using System.Collections.Generic;
using System.Linq;
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

            var customers = source.ForPage(query).AsEnumerable();

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

        public async Task<Customer> GetByTelephoneIdAsync(string telephoneId)
        {
            var id = DataProvider.Models.Telephone.DecomposeCustomerRecordId(telephoneId);

            var customer = await FindByDataIdAsync(id);

            if (customer == null)
            {
                _logger.LogError($"No customer found for data id {id}, extracted from telephone id {telephoneId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Customer>(customer);
        }

        public async Task<Customer> GetByRequestIdAsync(int requestId)
        {
            var customer = await _context.Requests.Where(r => r.Id == requestId).Select(r => r.Customer).FirstOrDefaultAsync();

            if (customer == null)
            {
                _logger.LogError($"No customer found for request id {requestId}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Customer>(customer);
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            var customerRecord = _mapper.Map<DataProvider.Models.Customer>(customer);

            customerRecord.Number = await _sequenceDataProvider.GetNextCustomerNumber();
            customerRecord.Created = DateTime.Now;
            customerRecord.SearchName = CalculateSearchName(customer.Name);

            await HydratePostalCode(customer, customerRecord);

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
            customerRecord.SearchName = CalculateSearchName(customer.Name);

            await HydratePostalCode(customer, customerRecord);

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
            var source = BaseQuery()
                            .Where(c => c.Number == number);

            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }

        private async Task<DataProvider.Models.Customer> FindByDataIdAsync(int id, QuerySet query = null)
        {
            var source = BaseQuery()
                            .Where(c => c.DataId == id);

            if (query != null)
                source = source.Include(query);

            return await source.FirstOrDefaultAsync();
        }
    }
}