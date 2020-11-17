using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<Paged<OutstandingJob>> GetOutstandingJobs(QuerySet querySet)
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

            var countSql = $@"
                SELECT COUNT(*)
                FROM tblOfferte o
                INNER JOIN tblData c ON c.ID = o.KlantID
                LEFT JOIN tblData g ON g.ID = o.GebouwID AND g.ParentID = c.ID
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

            var sql = $@"
                SELECT o.OfferteID as orderId, o.OfferteNr as offerNumber, r.AanvraagID as requestId, b.Bezoeker as visitor,
                    o.BestelDatum as orderDate, c.ID as customerNumber, c.Naam as customerName,
                    c.Adres1 as customerAddress1, c.Adres2 as customerAddress2, c.Adres3 as customerAddress3,
                    c.Postcode as customerPostalCode, c.Gemeente as customerCity, g.Naam as buildingName,
                    g.Adres1 as buildingAddress1, g.Adres2 as buildingAddress2, g.Adres3 as buildingAddress3,
                    g.Postcode as buildingPostalCode, g.Gemeente as buildingCity,
                    o.VastgelegdeDatum as planningDate, o.VerwachteDatum as expectedDate, o.VereisteDatum as requiredDate,
                    o.UrenGepland as scheduledNbOfHours, o.ManGepland as scheduledNbOfPersons,
                    o.Produktiebon as hasProductionTicket, o.Plaatsing as mustBeInstalled,
                    o.TeLeveren as mustBeDelivered, o.ProductKlaar as productIsReady
                FROM tblOfferte o
                INNER JOIN tblData c ON c.ID = o.KlantID
                LEFT JOIN tblData g ON g.ID = o.GebouwID AND g.ParentID = c.ID
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
                ORDER BY o.BestelDatum ASC
            ";

            _logger.LogInformation(sql);

            using (_dbConnection)
            {
                var count = _dbConnection.Query<int>(countSql).First();
                // TODO pagination should be embedded in the SQL query instead of taking a subset of the resultset,
                // but when JOINS are used in the SQL query this entails more than just adding 'OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY'
                var entries = _dbConnection.Query<OutstandingJob>(sql)
                                    .Skip(querySet.Page.Skip)
                                    .Take(querySet.Page.Take);
                return await Task.Run(() => new Paged<OutstandingJob>() {
                    Items = entries,
                    Count = count,
                    PageNumber = querySet.Page.Number,
                    PageSize = querySet.Page.Size
                });
            }
        }
    }
}