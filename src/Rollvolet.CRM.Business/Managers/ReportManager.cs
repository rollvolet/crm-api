using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Business.Managers.Interfaces;
using Rollvolet.CRM.Business.Models;

namespace Rollvolet.CRM.Business.Managers
{
    public class ReportManager : IReportManager
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger _logger;

        public ReportManager(IDbConnection dbConnection, ILogger<ReportManager> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public async Task<IEnumerable<MonthlySalesEntry>> GetMonthlySalesReport(int fromYear, int toYear)
        {
            var sql = @"
                SELECT r.month, r.year, SUM(r.netAmount) as amount
                FROM (
                        (
                        SELECT MONTH(c.Datum) as month, YEAR(c.Datum) as year, c.Bedrag * -1.0 as netAmount
                        FROM dbo.TblFactuur as c
                        WHERE c.CreditNota = 1 AND YEAR(c.Datum) >= @FromYear AND YEAR(c.Datum) <= @ToYear
                        )
                        UNION ALL
                        (
                        SELECT MONTH(f.Datum) as month, YEAR(f.Datum) as year, f.Bedrag as NetAmount
                        FROM dbo.TblFactuur as f
                        WHERE f.CreditNota = 0  AND YEAR(f.Datum) >= @FromYear AND YEAR(f.Datum) <= @ToYear
                        )
                ) as r
                GROUP BY r.month, r.year
                ORDER BY r.year, r.month
            ";

            using (_dbConnection)
            {
                var entries = _dbConnection.Query<MonthlySalesEntry>(sql, new { FromYear = fromYear, ToYear = toYear });
                return await Task.Run(() => entries);
            }
        }
    }
}