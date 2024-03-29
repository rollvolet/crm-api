using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class EmployeeManager : IEmployeeManager
    {
        private readonly IEmployeeDataProvider _employeeDataProvider;
        private readonly ILogger _logger;

        public EmployeeManager(IEmployeeDataProvider employeeDataProvider, ILogger<EmployeeManager> logger)
        {
            _employeeDataProvider = employeeDataProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _employeeDataProvider.GetAllAsync();
        }

        public async Task<Employee> GetByInterventionIdAsync(int interventionId)
        {
            return await _employeeDataProvider.GetByInterventionIdAsync(interventionId);
        }

        public async Task<Paged<Employee>> GetAllByInterventionIdAsync(int interventionId, QuerySet query)
        {
            return await _employeeDataProvider.GetAllByInterventionIdAsync(interventionId, query);
        }

        public async Task<Paged<Employee>> GetAllByOrderIdAsync(int orderId, QuerySet query)
        {
            return await _employeeDataProvider.GetAllByOrderIdAsync(orderId, query);
        }
    }
}