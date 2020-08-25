using System.Text.Json.Serialization;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class ContactAndBuildingDto
    {
        [JsonPropertyName("customerId")]
        public string CustomerId { get; set; }
        [JsonPropertyName("contactId")]
        public string ContactId { get; set; }
        [JsonPropertyName("buildingId")]
        public string BuildingId { get; set; }
        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }
        [JsonPropertyName("interventionId")]
        public string InterventionId { get; set; }
        [JsonPropertyName("offerId")]
        public string OfferId { get; set; }
        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }
        [JsonPropertyName("invoiceId")]
        public string InvoiceId { get; set; }
    }
}