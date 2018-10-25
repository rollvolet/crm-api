using System.IO;
using System.Threading.Tasks;

namespace Rollvolet.CRM.Domain.Managers.Interfaces
{
    public interface IDocumentGenerationManager
    {
        Task<Stream> CreateVisitReport(int requestId);
        Task<FileStream> CreateAndStoreOfferDocument(int offerId);
        Task<FileStream> DownloadOfferDocument(int offerId);
        Task<FileStream> CreateAndStoreInvoiceDocumentAsync(int offerId);
        Task<FileStream> DownloadInvoiceDocumentAsync(int offerId);
        Task UploadProductionTicket(int orderId, Stream content);
        Task<FileStream> DownloadProductionTicket(int orderId);
    }
}
