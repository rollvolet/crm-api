using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Customer>> GetAll()
        {
            var customers = await _context.Customers.ToListAsync();

            return _mapper.Map<IEnumerable<Customer>>(customers);
        }
    }
}