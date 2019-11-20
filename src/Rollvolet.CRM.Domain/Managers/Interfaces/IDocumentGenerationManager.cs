using System.IO;
using System.Threading.Tasks;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IDocumentGenerationManager
    {
        Task<Stream> CreateVisitReportAsync(int requestId);
        Task CreateAndStoreOfferDocumentAsync(int offerId);
        Task<FileStream> DownloadOfferDocumentAsync(int offerId);
        Task CreateAndStoreOrderDocumentAsync(int orderId);
        Task<FileStream> DownloadOrderDocumentAsync(int orderId);
        Task CreateAndStoreDeliveryNoteAsync(int orderId);
        Task<FileStream> DownloadDeliveryNoteAsync(int orderId);
        Task CreateAndStoreInvoiceDocumentAsync(int invoiceId, string language);
        Task<FileStream> DownloadInvoiceDocumentAsync(int invoiceId);
        Task CreateAndStoreDepositInvoiceDocumentAsync(int depositInvoiceId, string language);
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
