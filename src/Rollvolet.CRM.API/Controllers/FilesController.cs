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
            try
            {
              var fileStream = await _documentGenerationManager.DownloadOfferDocument(id);
              var file = new FileStreamResult(fileStream, "application/pdf");
              // file.FileDownloadName = fileStream.Name;  // this makes file being server with disposition=attachment
              return file;
            }
            catch (EntityNotFoundException)
            {
              return Ok("Document bestaat niet.");
            }
        }

        [HttpGet("invoices/{id}")]
        public async Task<IActionResult> ViewInvoiceDocumentAsync(int id)
        {
            var fileStream = await _documentGenerationManager.DownloadInvoiceDocumentAsync(id);
            var file = new FileStreamResult(fileStream, "application/pdf");
            // file.FileDownloadName = fileStream.Name;  // this makes file being server with disposition=attachment
            return file;
        }

        [HttpGet("deposit-invoices/{id}")]
        public async Task<IActionResult> ViewDepositInvoiceDocumentAsync(int id)
        {
            var fileStream = await _documentGenerationManager.DownloadDepositInvoiceDocumentAsync(id);
            var file = new FileStreamResult(fileStream, "application/pdf");
            // file.FileDownloadName = fileStream.Name;  // this makes file being server with disposition=attachment
            return file;
        }
    }
}