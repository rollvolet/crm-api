namespace Rollvolet.CRM.Domain.Configuration
{
    public class DocumentGenerationConfiguration
    {
        public string BaseUrl { get; set; }
        public string OfferStorageLocation { get; set; }
        public string OrderStorageLocation { get; set; }
        public string DeliveryNoteStorageLocation { get; set; }
        public string GeneratedProductionTicketStorageLocation { get; set; }
        public string ReceivedProductionTicketStorageLocation { get; set; }
        public string InvoiceStorageLocation { get; set; }
        public string GeneratedCertificateStorageLocation { get; set; }
        public string ReceivedCertificateStorageLocation { get; set; }
        public string CertificateUploadSourceLocation { get; set; }
    }
}