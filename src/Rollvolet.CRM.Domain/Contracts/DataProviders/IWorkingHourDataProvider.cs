using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Contracts.DataProviders
{
    public interface IWorkingHourDataProvider
    {
        Task<WorkingHour> GetByIdAsync(int id, QuerySet query = null);
        Task<Paged<WorkingHour>> GetAllByInvoiceIdAsync(int workingHourId, QuerySet query);
        Task<WorkingHour> CreateAsync(WorkingHour workingHour);
        Task<WorkingHour> UpdateAsync(WorkingHour workingHour);
        Task DeleteByIdAsync(int id);
    }
}