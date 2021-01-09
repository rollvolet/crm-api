using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IDocumentGenerationManager _documentGenerationManager;
        private readonly ILogger _logger;

        public FilesController(IDocumentGenerationManager documentGenerationManager, ILogger<FilesController> logger)
        {
            _documentGenerationManager = documentGenerationManager;
            _logger = logger;
        }

        [HttpGet("requests/{id}")]
        public async Task<IActionResult> ViewVisitReportAsync(int id)
        {
            return await ViewStreamAsync(_documentGenerationManager.DownloadVisitReportAsync, id);
        }

        [HttpGet("interventions/{id}")]
        public async Task<IActionResult> ViewInterventionReportAsync(int id)
        {
            return await ViewStreamAsync(_documentGenerationManager.DownloadInterventionReportAsync, id);
        }

        [HttpGet("offers/{id}")]
        public async Task<IActionResult> ViewOfferDocumentAsync(int id)
        {
            return await ViewStreamAsync(_documentGenerationManager.DownloadOfferDocumentAsync, id);
        }

        [HttpGet("orders/{id}")]
        public async Task<IActionResult> ViewOrderDocumentAsync(int id)
        {
            return await ViewStreamAsync(_documentGenerationManager.DownloadOrderDocumentAsync, id);
        }

        [HttpGet("delivery-notes/{orderId}")]
        public async Task<IActionResult> ViewDeliveryNoteAsync(int orderId)
        {
            return await ViewStreamAsync(_documentGenerationManager.DownloadDeliveryNoteAsync, orderId);
        }

        [HttpGet("production-ticket-templates/{orderId}")]
        public async Task<IActionResult> ViewProductionTicketTemplateAsync(int orderId)
        {
            return await ViewStreamAsync(_documentGenerationManager.DownloadProductionTicketTemplateAsync, orderId);
        }

        [HttpGet("production-tickets/{orderId}")]
        public async Task<IActionResult> ViewProductionTicketAsync(int orderId, [FromQuery] bool watermark)
        {
            if (watermark)
                return await ViewStreamAsync(_documentGenerationManager.DownloadProductionTicketWithWatermarkAsync, orderId);
            else
                return await ViewStreamAsync(_documentGenerationManager.DownloadProductionTicketAsync, orderId);
        }

        [HttpGet("invoices/{id}")]
        public async Task<IActionResult> ViewInvoiceDocumentAsync(int id)
        {
            return await ViewStreamAsync(_documentGenerationManager.DownloadInvoiceDocumentAsync, id);
        }

        [HttpGet("invoices/{id}/certificate-template")]
        public async Task<IActionResult> ViewVatCertificateTemplateForInvoiceAsync(int id)
        {
            return await ViewStreamAsync(_documentGenerationManager.DownloadCertificateTemplateForInvoiceAsync, id);
        }

        [HttpGet("invoices/{id}/certificate")]
        public async Task<IActionResult> ViewVatCertificateForInvoiceAsync(int id)
        {
            return await ViewStreamAsync(_documentGenerationManager.DownloadCertificateForInvoiceAsync, id);
        }

        [HttpGet("deposit-invoices/{id}")]
        public async Task<IActionResult> ViewDepositInvoiceDocumentAsync(int id)
        {
            return await ViewStreamAsync(_documentGenerationManager.DownloadDepositInvoiceDocumentAsync, id);
        }

        [HttpGet("deposit-invoices/{id}/certificate-template")]
        public async Task<IActionResult> ViewVatCertificateTemplateForDepositInvoiceAsync(int id)
        {
            return await ViewStreamAsync(_documentGenerationManager.DownloadCertificateTemplateForDepositInvoiceAsync, id);
        }

        [HttpGet("deposit-invoices/{id}/certificate")]
        public async Task<IActionResult> ViewVatCertificateForDepositInvoiceAsync(int id)
        {
            return await ViewStreamAsync(_documentGenerationManager.DownloadCertificateForDepositInvoiceAsync, id);
        }

        private async Task<IActionResult> ViewStreamAsync<T>(Func<int, Task<T>> downloadFileAsync, int id) where T : Stream
        {
            try
            {
                var fileStream = await downloadFileAsync(id);
                var file = new FileStreamResult(fileStream, "application/pdf");
                // file.FileDownloadName = fileStream.Name;  // this makes file being served with disposition=attachment
                return file;
            }
            catch (EntityNotFoundException)
            {
                return Ok("Document bestaat niet.");
            }
        }
    }
}