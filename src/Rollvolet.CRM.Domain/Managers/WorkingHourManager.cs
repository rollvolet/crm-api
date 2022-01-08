using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class WorkingHourManager : IWorkingHourManager
    {
        private readonly IWorkingHourDataProvider _workingHourDataProvider;
        private readonly IInvoiceDataProvider _invoiceDataProvider;
        private readonly IEmployeeDataProvider _employeeDataProvider;
        private readonly ILogger _logger;

        public WorkingHourManager(IWorkingHourDataProvider workingHourDataProvider, IInvoiceDataProvider invoiceDataProvider,
                                    IEmployeeDataProvider employeeDataProvider, ILogger<InvoiceManager> logger)
        {
            _workingHourDataProvider = workingHourDataProvider;
            _invoiceDataProvider = invoiceDataProvider;
            _employeeDataProvider = employeeDataProvider;
            _logger = logger;
        }

        public async Task<Paged<WorkingHour>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query)
        {
            if (query.Sort.Field == null) {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "date";
            }

            return await _workingHourDataProvider.GetAllByInvoiceIdAsync(invoiceId, query);
        }

        public async Task<WorkingHour> CreateAsync(WorkingHour workingHour)
        {
            if (workingHour.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Working-hour cannot have an id on create.");
            if (workingHour.Date == null)
                throw new IllegalArgumentException("IllegalAttribute", "Date is required on working hour.");
            if (workingHour.Invoice == null)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice is required on working hour creation.");
            if (workingHour.Employee == null)
                throw new IllegalArgumentException("IllegalAttribute", "Employee is required on working hour creation.");

            await EmbedRelationsAsync(workingHour);

            return await _workingHourDataProvider.CreateAsync(workingHour);
        }

        public async Task<WorkingHour> UpdateAsync(WorkingHour workingHour)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "invoice", "employee" };
            var existingWorkingHour = await _workingHourDataProvider.GetByIdAsync(workingHour.Id, query);

            if (workingHour.Id != existingWorkingHour.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Working-hour id cannot be updated.");
            if (workingHour.Date == null)
                throw new IllegalArgumentException("IllegalAttribute", "Date is required on working hour.");

            await EmbedRelationsAsync(workingHour, existingWorkingHour);

            if (workingHour.Invoice == null)
                throw new IllegalArgumentException("IllegalAttribute", "Invoice is required.");
            if (workingHour.Employee == null)
                throw new IllegalArgumentException("IllegalAttribute", "Employee is required.");

            return await _workingHourDataProvider.UpdateAsync(workingHour);
        }

        public async Task DeleteAsync(int id)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "invoice" };
            var workingHour = await _workingHourDataProvider.GetByIdAsync(id, query);

            await _workingHourDataProvider.DeleteByIdAsync(id);
        }

        // Embed relations in working-hour resource: reuse old relation if there is one and it hasn't changed
        private async Task EmbedRelationsAsync(WorkingHour workingHour, WorkingHour oldWorkingHour = null)
        {
        try {
                if (oldWorkingHour != null)
                    workingHour.Invoice = oldWorkingHour.Invoice; // invoice cannot be updated. Take invoice of old workingHour on update.
                else
                    workingHour.Invoice = await _invoiceDataProvider.GetByIdAsync(workingHour.Invoice.Id);

                if (oldWorkingHour != null && oldWorkingHour.Employee != null && oldWorkingHour.Employee.Id == oldWorkingHour.Employee.Id)
                    workingHour.Employee = oldWorkingHour.Employee;
                else
                    workingHour.Employee = await _employeeDataProvider.GetByIdAsync(workingHour.Employee.Id);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }
    }
}