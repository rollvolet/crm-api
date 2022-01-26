using System.IO;
using System.Threading.Tasks;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IDocumentGenerationManager
    {
        Task CreateAndStoreVisitReportAsync(int requestId);
        Task<Stream> DownloadVisitReportAsync(int requestId);
        Task DeleteVisitReportAsync(int requestId);
        Task CreateAndStoreInterventionReportAsync(int interventionId);
        Task<Stream> DownloadInterventionReportAsync(int interventionId);
        Task DeleteInterventionReportAsync(int interventionId);
        Task CreateAndStoreOfferDocumentAsync(int offerId);
        Task<Stream> DownloadOfferDocumentAsync(int offerId);
        Task DeleteOfferDocumentAsync(int offerId);
        Task CreateAndStoreOrderDocumentAsync(int orderId);
        Task<Stream> DownloadOrderDocumentAsync(int orderId);
        Task DeleteOrderDocumentAsync(int orderId);
        Task CreateAndStoreDeliveryNoteAsync(int orderId);
        Task<Stream> DownloadDeliveryNoteAsync(int orderId);
        Task DeleteDeliveryNoteAsync(int orderId);
        Task CreateAndStoreProductionTicketTemplateAsync(int orderId);
        Task<Stream> DownloadProductionTicketTemplateAsync(int orderId);
        Task DeleteProductionTicketTemplateAsync(int orderId);
        Task UploadProductionTicketAsync(int orderId, Stream content);
        Task<Stream> DownloadProductionTicketAsync(int orderId);
        Task DeleteProductionTicketAsync(int orderId);
        Task<Stream> DownloadProductionTicketWithWatermarkAsync(int orderId);
        Task CreateAndStoreInvoiceDocumentAsync(int invoiceId);
        Task<Stream> DownloadInvoiceDocumentAsync(int invoiceId);
        Task DeleteInvoiceDocumentAsync(int invoiceId);
        Task CreateAndStoreDepositInvoiceDocumentAsync(int depositInvoiceId);
        Task<Stream> DownloadDepositInvoiceDocumentAsync(int depositInvoiceId);
        Task DeleteDepositInvoiceDocumentAsync(int depositInvoiceId);
    }
}
