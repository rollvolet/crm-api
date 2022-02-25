using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Business.Models;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Business.Managers.Interfaces
{
    public interface IReportManager
    {
        Task<IEnumerable<MonthlySalesEntry>> GetMonthlySalesReport(int fromYear, int toYear);
        Task<AverageDurationReport> GetAverageDurationReport(int nbOfCases);
        Task<Paged<OutstandingJob>> GetOutstandingJobs(QuerySet querySet);
        Task<OutstandingJobReport> GetOutstandingJobReport(QuerySet querySet);
    }
}
