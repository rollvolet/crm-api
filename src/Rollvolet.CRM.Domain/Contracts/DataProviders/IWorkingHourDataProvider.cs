using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IWorkingHourDataProvider
    {
        Task<Paged<WorkingHour>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query);
    }
}