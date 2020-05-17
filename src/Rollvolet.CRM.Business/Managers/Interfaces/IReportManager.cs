using System.Collections.Generic;
using System.Threading.Tasks;
using Rollvolet.CRM.Business.Models;

namespace Rollvolet.CRM.Business.Managers.Interfaces
{
    public interface IReportManager
    {
        Task<IEnumerable<MonthlySalesEntry>> GetMonthlySalesReport(int fromYear, int toYear);
    }
}
