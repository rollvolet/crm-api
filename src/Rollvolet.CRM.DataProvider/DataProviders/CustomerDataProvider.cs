using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Narato.ResponseMiddleware.Models.Exceptions;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;

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
            var skip = (query.Page.Number - 1) * query.Page.Size;
            var take = query.Page.Size;

            var source = _context.Customers;

            var customers = await source.Include(c => c.Country).Skip(skip).Take(take).ToListAsync();
            var count = await source.CountAsync();

            var mappedCustomers = _mapper.Map<IEnumerable<Customer>>(customers);

            return new Paged<Customer>() {
                Items = mappedCustomers,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            var customer = await _context.Customers.Where(c => c.CustomerId == id).FirstOrDefaultAsync();
            
            if (customer == null)
            {
                // TODO implement and handle exceptions according to jsonapi
                throw new EntityNotFoundException("ENF", $"Customer with id {id} not found");
            }

            return _mapper.Map<Customer>(customer);
        }
    }
}