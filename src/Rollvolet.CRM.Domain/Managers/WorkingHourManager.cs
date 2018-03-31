using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class WorkingHourManager : IWorkingHourManager
    {
        private readonly IWorkingHourDataProvider _workingHourDataProvider;

        public WorkingHourManager(IWorkingHourDataProvider workingHourDataProvider)
        {
            _workingHourDataProvider = workingHourDataProvider;
        }
        
        public async Task<Paged<WorkingHour>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query)
        {
            if (query.Sort.Field == null) {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "date";
            }

            return await _workingHourDataProvider.GetAllByInvoiceIdAsync(invoiceId, query);
        }
    }
}