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
    [Route("[controller]")]
    [AllowAnonymous]
    public class FilesController : ControllerBase
    {
        private readonly IDocumentGenerationManager _documentGenerationManager;
        private readonly ILogger _logger;

        public FilesController(IDocumentGenerationManager documentGenerationManager, ILogger<FilesController> logger)
        {
            _documentGenerationManager = documentGenerationManager;
            _logger = logger;
        }

        [HttpGet("offers/{id}")]
        public async Task<IActionResult> ViewOfferDocumentAsync(int id)
        {
            return await ViewFileStream(_documentGenerationManager.DownloadOfferDocumentAsync, id);
        }

        [HttpGet("orders/{id}")]
        public async Task<IActionResult> ViewOrderDocumentAsync(int id)
        {
            return await ViewFileStream(_documentGenerationManager.DownloadOrderDocumentAsync, id);
        }

        [HttpGet("delivery-notes/{orderId}")]
        public async Task<IActionResult> ViewDeliveryNoteAsync(int orderId)
        {
            return await ViewFileStream(_documentGenerationManager.DownloadDeliveryNoteAsync, orderId);
        }

        [HttpGet("invoices/{id}")]
        public async Task<IActionResult> ViewInvoiceDocumentAsync(int id)
        {
            return await ViewFileStream(_documentGenerationManager.DownloadInvoiceDocumentAsync, id);
        }

        [HttpGet("deposit-invoices/{id}")]
        public async Task<IActionResult> ViewDepositInvoiceDocumentAsync(int id)
        {
            return await ViewFileStream(_documentGenerationManager.DownloadDepositInvoiceDocumentAsync, id);
        }

        private async Task<IActionResult> ViewFileStream(Func<int, Task<FileStream>> downloadFileAsync, int id)
        {
            try
            {
                var fileStream = await downloadFileAsync(id);
                var file = new FileStreamResult(fileStream, "application/pdf");
                // file.FileDownloadName = fileStream.Name;  // this makes file being server with disposition=attachment
                return file;
            }
            catch (EntityNotFoundException)
            {
                return Ok("Document bestaat niet.");
            }
        }
    }
}