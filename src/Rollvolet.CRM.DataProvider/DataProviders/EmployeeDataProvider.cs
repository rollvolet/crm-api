using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.DataProviders
{
    public class EmployeeDataProvider : IEmployeeDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EmployeeDataProvider(CrmContext context, IMapper mapper, ILogger<EmployeeDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Employee>> GetAll()
        {
            var employees = _context.Employees.OrderBy(c => c.FirstName).AsEnumerable();

            return _mapper.Map<IEnumerable<Employee>>(employees);
        }

        public async Task<Employee> GetByFirstName(string name)
        {
            var employee = await _context.Employees.Where(c => c.FirstName == name).FirstOrDefaultAsync();

            if (employee == null)
            {
                _logger.LogError($"No employee found with first-name {name}");
                throw new EntityNotFoundException();
            }

            return _mapper.Map<Employee>(employee);
        }

        public async Task<Employee> GetVisitorByOfferId(int offerId)
        {
            var visit = await _context.Offers.Where(o => o.Id == offerId).Take(1).Select(o => o.Request).Select(r => r.Visit).FirstOrDefaultAsync();

            if (visit == null)
            {
                _logger.LogError($"No employee found for order {offerId}");
                throw new EntityNotFoundException();
            }

            return await GetByFirstName(visit.Visitor);
        }

        public async Task<Employee> GetVisitorByOrderId(int orderId)
        {
            var visit = await _context.Orders.Where(o => o.Id == orderId).Take(1)
                                .Select(o => o.Offer).Select(o => o.Request).Select(r => r.Visit).FirstOrDefaultAsync();

            if (visit == null)
            {
                _logger.LogError($"No employee found for order {orderId}");
                throw new EntityNotFoundException();
            }

            return await GetByFirstName(visit.Visitor);
        }
    }
}