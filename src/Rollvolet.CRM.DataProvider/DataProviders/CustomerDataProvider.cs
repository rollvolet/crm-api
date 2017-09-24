using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Narato.ResponseMiddleware.Models.Exceptions;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.DataProvider.Extensions;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.DataProviders
{   
    public class CustomerDataProvider : ICustomerDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;

        public CustomerDataProvider(CrmContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Paged<Customer>> GetAllAsync(QuerySet query)
        {
            var source = _context.Customers
                            .Include(query)
                            .Sort(query);

            var customers = source.Skip(query.Page.Skip).Take(query.Page.Take).AsEnumerable();

            var mappedCustomers = _mapper.Map<IEnumerable<Customer>>(customers);

            var count = await source.CountAsync();

            return new Paged<Customer>() {
                Items = mappedCustomers,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Customer> GetByNumberAsync(int number, QuerySet query)
        {
            var source = _context.Customers
                            .Where(c => c.Number == number)
                            .Include(query);

            var customer = await source.FirstOrDefaultAsync();
            
            if (customer == null)
            {
                // TODO implement and handle exceptions according to jsonapi
                throw new EntityNotFoundException("ENF", $"Customer with number {number} not found");
            }

            return _mapper.Map<Customer>(customer);
        }

        public async Task<Customer> Create(Customer customer)
        {
            var customerRecord = _mapper.Map<DataProvider.Models.Customer>(customer);

            // TODO auto-fill properties: embeddedPostalCode, embeddedCity, searchName

            _context.Customers.Add(customerRecord);
            await _context.SaveChangesAsync();

            return _mapper.Map<Customer>(customerRecord); // id is filled in now
        }
    }
}