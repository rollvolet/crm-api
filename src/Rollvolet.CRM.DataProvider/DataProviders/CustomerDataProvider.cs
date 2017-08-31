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

            var sourceQuery = AddIncludeClauses(source, query);
            var customers = await sourceQuery.Skip(skip).Take(take).ToListAsync();
            var mappedCustomers = _mapper.Map<IEnumerable<Customer>>(customers);

            var count = await source.CountAsync();

            return new Paged<Customer>() {
                Items = mappedCustomers,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };
        }

        public async Task<Customer> GetByIdAsync(int id, QuerySet query)
        {
            var source = _context.Customers.Where(c => c.AlternateId == id);
            var sourceQuery = AddIncludeClauses(source, query);
            var customer = await sourceQuery.FirstOrDefaultAsync();
            
            if (customer == null)
            {
                // TODO implement and handle exceptions according to jsonapi
                throw new EntityNotFoundException("ENF", $"Customer with id {id} not found");
            }

            return _mapper.Map<Customer>(customer);
        }

        private IQueryable<DataProvider.Models.Customer> AddIncludeClauses(IQueryable<DataProvider.Models.Customer> sourceQuery, QuerySet query)
        {
            foreach (var field in query.Include.Fields)
            {
                if ("country".Equals(field))
                    sourceQuery = sourceQuery.Include(c => c.Country);

                if ("contacts".Equals(field))
                    sourceQuery = sourceQuery.Include(c => c.Contacts);

                if ("buildings".Equals(field))
                    sourceQuery = sourceQuery.Include(c => c.Buildings);
            }

            return sourceQuery;
        }
    }
}