namespace Rollvolet.CRM.Domain.Configuration
{
    public class DocumentGenerationConfiguration
    {
        public string BaseUrl { get; set; }
        public string OfferStorageLocation { get; set; }
        public string OrderStorageLocation { get; set; }
        public string DeliveryNoteStorageLocation { get; set; }
        public string InvoiceStorageLocation { get; set; }
        public string ProductionTicketStorageLocation { get; set; }
        public string GeneratedCertificateStorageLocation { get; set; }
        public string ReceivedCertificateStorageLocation { get; set; }
    }
}