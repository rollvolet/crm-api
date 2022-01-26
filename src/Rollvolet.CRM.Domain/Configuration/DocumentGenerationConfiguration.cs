namespace Rollvolet.CRM.Domain.Configuration
{
    public class DocumentGenerationConfiguration
    {
        public string BaseUrl { get; set; }
        public bool? DisableSearch { get; set; }
        public string VisitReportStorageLocation { get; set; }
        public string InterventionReportStorageLocation { get; set; }
        public string OfferStorageLocation { get; set; }
        public string OrderStorageLocation { get; set; }
        public string DeliveryNoteStorageLocation { get; set; }
        public string GeneratedProductionTicketStorageLocation { get; set; }
        public string ReceivedProductionTicketStorageLocation { get; set; }
        public string InvoiceStorageLocation { get; set; }

        public bool IsSearchEnabled
        {
            get
            {
                return DisableSearch == null || DisableSearch == false;
            }
        }
    }
}