using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Business.Managers.Interfaces;
using Rollvolet.CRM.Business.Models;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

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

        public async Task<OutstandingJobReport> GetOutstandingJobReport(QuerySet querySet)
        {
            var whereClause = GetOutstandingJobsSqlWhereClause(querySet);

            var totalHoursSql = $@"
                SELECT COALESCE(SUM(o.UrenGepland * o.ManGepland), 0) as totalHours
                {whereClause}
            ";

            var numberOverdueSql = $@"
                SELECT o.OfferteID as orderId, o.VastgelegdeDatum as planningDateStr, 
                o.VerwachteDatum as expectedDateStr, o.VereisteDatum as requiredDateStr
                {whereClause}
            ";

            using (_dbConnection)
            {
                var totalHours = _dbConnection.Query<double>(totalHoursSql).First();

                // TODO count overdue jobs using an SQL query once the required date
                // is stored as datetime instead of string in the DB 
                var types = new[] { typeof(OutstandingJob), typeof(String), typeof(String), typeof(String) };
                Func<object[], OutstandingJob> mapper = GetOutstandingJobMapper();
                var entries = _dbConnection.Query<OutstandingJob>(numberOverdueSql, types, mapper, splitOn: "planningDateStr,expectedDateStr,requiredDateStr");
                var numberOverdue = entries.Where(o => o.RequiredDate <= DateTime.Now).Count();

                return await Task.Run(() => new OutstandingJobReport() {
                    TotalHours = totalHours,
                    NumberOverdue = numberOverdue
                });
            }
        }

        public async Task<Paged<OutstandingJob>> GetOutstandingJobs(QuerySet querySet)
        {
            var whereClause = GetOutstandingJobsSqlWhereClause(querySet);

            var countSql = $@"
                SELECT COUNT(*)
                {whereClause}
            ";

            var sql = $@"
                SELECT o.OfferteID as orderId, o.OfferteNr as offerNumber, r.AanvraagID as requestId, b.Bezoeker as visitor,
                    o.BestelDatum as orderDate, c.ID as customerNumber, c.Naam as customerName,
                    c.Adres1 as customerAddress1, c.Adres2 as customerAddress2, c.Adres3 as customerAddress3,
                    c.Postcode as customerPostalCode, c.Gemeente as customerCity, g.Naam as buildingName,
                    g.Adres1 as buildingAddress1, g.Adres2 as buildingAddress2, g.Adres3 as buildingAddress3,
                    g.Postcode as buildingPostalCode, g.Gemeente as buildingCity,
                    o.UrenGepland as scheduledNbOfHours, o.ManGepland as scheduledNbOfPersons,
                    o.Produktiebon as hasProductionTicket, o.Plaatsing as mustBeInstalled,
                    o.TeLeveren as mustBeDelivered, o.ProductKlaar as productIsReady, o.Opmerking as comment,
                    o.VastgelegdeDatum as planningDateStr, o.VerwachteDatum as expectedDateStr, o.VereisteDatum as requiredDateStr,
                    (
                        SELECT STRING_AGG(t.Voornaam, ', ') WITHIN GROUP (ORDER BY t.Voornaam ASC)
                        FROM TblOrderTechnician ot
                        LEFT JOIN TblPersoneel t ON ot.EmployeeId = t.PersoneelId
                        WHERE ot.OrderId = o.OfferteID
                        GROUP BY ot.OrderId
                    ) as technicians
                {whereClause}
            ";

            _logger.LogInformation(sql);

            var types = new[] { typeof(OutstandingJob), typeof(String), typeof(String), typeof(String) };
            Func<object[], OutstandingJob> mapper = GetOutstandingJobMapper();

            var sortField = querySet.Sort.Field;
            Func<OutstandingJob, DateTime?> sort = null;
            switch (sortField)
            {
                case "expected-date":
                    sort = (job) => job.ExpectedDate;
                    break;
                case "required-date":
                    sort = (job) => job.RequiredDate;
                    break;
                case "planning-date":
                    sort = (job) => job.PlanningDate;
                    break;
                default:
                    sort = (job) => job.OrderDate;
                    break;
            }

            using (_dbConnection)
            {
                var count = _dbConnection.Query<int>(countSql).First();
                // TODO pagination and sorting should be embedded in the SQL query instead of taking a subset of the resultset,
                // but when JOINS are used in the SQL query this entails more than just adding 'OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY'
                // TODO sorting on planning/expected/required date can only be done once the values are stored 
                // as dateTime instead of string in the database
                var entries = _dbConnection.Query<OutstandingJob>(sql, types, mapper, splitOn: "planningDateStr,expectedDateStr,requiredDateStr");
                entries = querySet.Sort.IsAscending ? entries.OrderBy(sort) : entries.OrderByDescending(sort);
                entries = entries.Skip(querySet.Page.Skip).Take(querySet.Page.Take);

                return await Task.Run(() => new Paged<OutstandingJob>() {
                    Items = entries,
                    Count = count,
                    PageNumber = querySet.Page.Number,
                    PageSize = querySet.Page.Size
                });
            }
        }

        private string GetOutstandingJobsSqlWhereClause(QuerySet querySet)
        {
            var filterFields = querySet.Filter.Fields;
            var filters = new List<string>();
            if (filterFields.ContainsKey("visitor"))
                filters.Add($" AND b.Bezoeker = '{filterFields["visitor"].Replace("'", "''")}'");
            if (filterFields.ContainsKey("hasProductionTicket") && Int32.Parse(filterFields["hasProductionTicket"]) >= 0)
                filters.Add($" AND o.Produktiebon = {Int32.Parse(filterFields["hasProductionTicket"])}");
            if (filterFields.ContainsKey("mustBeInstalled") && Int32.Parse(filterFields["mustBeInstalled"]) >= 0)
                filters.Add($" AND o.Plaatsing = {Int32.Parse(filterFields["mustBeInstalled"])}");
            if (filterFields.ContainsKey("mustBeDelivered") && Int32.Parse(filterFields["mustBeDelivered"]) >= 0)
                filters.Add($" AND o.TeLeveren = {Int32.Parse(filterFields["mustBeDelivered"])}");
            if (filterFields.ContainsKey("isProductReady") && Int32.Parse(filterFields["isProductReady"]) >= 0)
                filters.Add($" AND o.ProductKlaar = {Int32.Parse(filterFields["isProductReady"])}");
            if (filterFields.ContainsKey("orderDate")) // order date format yyyy-mm-dd
                filters.Add($" AND o.BestelDatum >= '{filterFields["orderDate"].Replace("'", "''")} 00:00:00'");

            var filterQuery = String.Join("", filters);

            return $@"
                FROM tblOfferte o
                INNER JOIN tblData c ON c.ID = o.KlantID AND c.DataType = 'KLA'
                LEFT JOIN tblData g ON g.ID = o.GebouwID AND g.ParentID = c.ID AND g.DataType = 'GEB'
                LEFT JOIN TblFactuur f ON f.OfferteID = o.OfferteID
                INNER JOIN TblAanvraag r ON o.AanvraagId = r.AanvraagID
                LEFT JOIN TblBezoek b ON b.AanvraagId = r.AanvraagID
                WHERE
                    o.Afgesloten = 0
                    AND o.AfgeslotenBestelling = 0
                    AND o.Besteld = 1
                    AND f.OfferteID IS NULL
                    AND o.MuntBestel = 'EUR'
                    {filterQuery}
            ";            
        }

        private Func<object[], OutstandingJob> GetOutstandingJobMapper()
        {
            // var types = new[] { typeof(OutstandingJob), typeof(String), typeof(String), typeof(String) };
            return (objects) =>
            {
                var outstandingJob = (OutstandingJob) objects[0];
                outstandingJob.PlanningDate = ParseDate((string) objects[1]);
                outstandingJob.ExpectedDate = ParseDate((string) objects[2]);
                outstandingJob.RequiredDate = ParseDate((string) objects[3]);
                return outstandingJob;
            };
        }

        private DateTime? ParseDate(string dateString)
        {
            if (dateString == null)
            {
                return null;
            }
            else
            {
                DateTime dateTime;
                if (DateTime.TryParseExact(dateString, "d/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    return dateTime;
                else if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    return dateTime;
                else if (DateTime.TryParseExact(dateString, "d/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    return dateTime;
                else
                    return null;
            }
        }        
    }
}
