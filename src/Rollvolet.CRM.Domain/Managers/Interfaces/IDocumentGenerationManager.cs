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
        Task<FileStream> DownloadInvoiceDocumentAsync(int offerId);
        Task<Stream> CreateCertificateAsync(int invoiceId, string language);
        Task UploadCertificateAsync(int invoiceId, Stream content);
        Task<FileStream> DownloadCertificateAsync(int invoiceId);
        Task UploadProductionTicketAsync(int orderId, Stream content);
        Task<FileStream> DownloadProductionTicketAsync(int orderId);
    }
}
