using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Business.Managers.Interfaces;
using Rollvolet.CRM.Domain.Managers.Interfaces;

namespace Rollvolet.CRM.Business.Managers
{
    public class SystemTaskExecutor : ISystemTaskExecutor
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDocumentGenerationManager _documentGenerationManager;
        private readonly ILogger _logger;

        public SystemTaskExecutor(IDbConnection dbConnection, IDocumentGenerationManager documentGenerationManager,
                                    ILogger<SystemTaskExecutor> logger)
        {
            _documentGenerationManager = documentGenerationManager;
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public async Task RestoreVatCertificates()
        {
            var sql = @"
                SELECT f.FactuurId as targetId, f.Nummer as targetNumber, f.KlantNaam as name,
                       org.FactuurId as sourceId, org.Nummer as sourceNumber
                FROM TblFactuur f
                INNER JOIN TblFactuur org ON f.AttestLink = org.Nummer
                WHERE f.AttestLink IS NOT NULL
            ";

            using (_dbConnection)
            {
                var entries = _dbConnection.Query(sql).ToList();
                _logger.LogInformation($"Start restoring VAT certificates for {entries.Count} invoices");
                var groups = entries.GroupBy(e => e.sourceId);
                foreach (var group in groups)
                {
                    _logger.LogInformation($"Restoring VAT certificates having A0{group.Key} as source.");

                    var isDepositSql = @"SELECT COUNT(*) FROM TblVoorschotFactuur f WHERE f.VoorschotFactuurID = @InvoiceId";
                    var isDepositInvoiceSource = _dbConnection.Query<int>(isDepositSql, new { InvoiceId = group.Key }).First() > 0;

                    foreach (var entry in group)
                    {
                        try
                        {
                            var isDepositInvoiceTarget = _dbConnection.Query<int>(isDepositSql, new { InvoiceId = entry.targetId }).First() > 0;

                            if (isDepositInvoiceTarget)
                            {
                                await _documentGenerationManager.RecycleCertificateForDepositInvoiceAsync((int) entry.targetId, (int) entry.sourceId, isDepositInvoiceSource);
                            }
                            else
                            {
                                await _documentGenerationManager.RecycleCertificateForInvoiceAsync((int) entry.targetId, (int) entry.sourceId, isDepositInvoiceSource);
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogWarning($"Failed to recycle VAT certificate for invoice {entry.targetId}: {e}");
                        }
                    }
                }
            }
        }
    }
}