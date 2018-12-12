using System.IO;
using System.Threading.Tasks;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IDocumentGenerationManager
    {
        Task<Stream> CreateVisitReportAsync(int requestId);
        Task<FileStream> CreateAndStoreOfferDocumentAsync(int offerId);
        Task<FileStream> DownloadOfferDocument(int offerId);
        Task<FileStream> CreateAndStoreInvoiceDocumentAsync(int invoiceId, string language);
        Task<FileStream> DownloadInvoiceDocumentAsync(int invoiceId);
        Task<FileStream> CreateAndStoreDepositInvoiceDocumentAsync(int depositInvoiceId, string language);
        Task<FileStream> DownloadDepositInvoiceDocumentAsync(int depositInvoiceId);
        Task<Stream> CreateCertificateForInvoiceAsync(int invoiceId, string language);
        Task UploadCertificateForInvoiceAsync(int invoiceId, Stream content);
        Task<FileStream> DownloadCertificateForInvoiceAsync(int invoiceId);
        Task<Stream> CreateCertificateForDepositInvoiceAsync(int invoiceId, string language);
        Task UploadCertificateForDepositInvoiceAsync(int invoiceId, Stream content);
        Task<FileStream> DownloadCertificateForDepositInvoiceAsync(int invoiceId);
        Task UploadProductionTicketAsync(int orderId, Stream content);
        Task<FileStream> DownloadProductionTicketAsync(int orderId);
    }
}
