using System.IO;
using System.Threading.Tasks;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IDocumentGenerationManager
    {
        Task CreateAndStoreVisitReportAsync(int requestId);
        Task<FileStream> DownloadVisitReportAsync(int requestId);
        Task DeleteVisitReportAsync(int requestId);
        Task CreateAndStoreInterventionReportAsync(int interventionId);
        Task<FileStream> DownloadInterventionReportAsync(int interventionId);
        Task DeleteInterventionReportAsync(int interventionId);
        Task CreateAndStoreOfferDocumentAsync(int offerId);
        Task<FileStream> DownloadOfferDocumentAsync(int offerId);
        Task DeleteOfferDocumentAsync(int offerId);
        Task CreateAndStoreOrderDocumentAsync(int orderId);
        Task<FileStream> DownloadOrderDocumentAsync(int orderId);
        Task DeleteOrderDocumentAsync(int orderId);
        Task CreateAndStoreDeliveryNoteAsync(int orderId);
        Task<FileStream> DownloadDeliveryNoteAsync(int orderId);
        Task DeleteDeliveryNoteAsync(int orderId);
        Task CreateAndStoreProductionTicketTemplateAsync(int orderId);
        Task<FileStream> DownloadProductionTicketTemplateAsync(int orderId);
        Task DeleteProductionTicketTemplateAsync(int orderId);
        Task UploadProductionTicketAsync(int orderId, Stream content);
        Task<FileStream> DownloadProductionTicketAsync(int orderId);
        Task DeleteProductionTicketAsync(int orderId);
        Task<Stream> DownloadProductionTicketWithWatermarkAsync(int orderId);
        Task CreateAndStoreInvoiceDocumentAsync(int invoiceId);
        Task<FileStream> DownloadInvoiceDocumentAsync(int invoiceId);
        Task DeleteInvoiceDocumentAsync(int invoiceId);
        Task CreateAndStoreDepositInvoiceDocumentAsync(int depositInvoiceId);
        Task<FileStream> DownloadDepositInvoiceDocumentAsync(int depositInvoiceId);
        Task DeleteDepositInvoiceDocumentAsync(int depositInvoiceId);
        Task CreateCertificateTemplateForInvoiceAsync(int invoiceId);
        Task<FileStream> DownloadCertificateTemplateForInvoiceAsync(int invoiceId);
        Task DeleteCertificateTemplateForInvoiceAsync(int invoiceId);
        Task UploadCertificateForInvoiceAsync(int invoiceId, Stream content, string uploadFileName = null);
        Task RecycleCertificateForInvoiceAsync(int invoiceId, int sourceInvoiceId, bool isDeposit);
        Task<FileStream> DownloadCertificateForInvoiceAsync(int invoiceId);
        Task DeleteCertificateForInvoiceAsync(int invoiceId);
        Task CreateCertificateTemplateForDepositInvoiceAsync(int invoiceId);
        Task<FileStream> DownloadCertificateTemplateForDepositInvoiceAsync(int invoiceId);
        Task DeleteCertificateTemplateForDepositInvoiceAsync(int invoiceId);
        Task UploadCertificateForDepositInvoiceAsync(int invoiceId, Stream content, string uploadFileName = null);
        Task RecycleCertificateForDepositInvoiceAsync(int invoiceId, int sourceInvoiceId, bool isDeposit);
        Task<FileStream> DownloadCertificateForDepositInvoiceAsync(int invoiceId);
        Task DeleteCertificateForDepositInvoiceAsync(int invoiceId);
    }
}
