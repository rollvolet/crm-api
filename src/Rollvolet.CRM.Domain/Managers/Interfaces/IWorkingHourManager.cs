using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IWorkingHourManager
    {
        Task<Paged<WorkingHour>> GetAllByInvoiceIdAsync(int invoiceId, QuerySet query);
        Task<WorkingHour> CreateAsync(WorkingHour workingHour);
        Task<WorkingHour> UpdateAsync(WorkingHour workingHour);
        Task DeleteAsync(int id);
    }
}
