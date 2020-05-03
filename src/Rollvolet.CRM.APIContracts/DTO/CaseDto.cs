using System.Text.Json.Serialization;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class CaseDto
    {
        [JsonPropertyName("customerId")]
        public int? CustomerId { get; set; }
        [JsonPropertyName("contactId")]
        public int? ContactId { get; set; }
        [JsonPropertyName("buildingId")]
        public int? BuildingId { get; set; }
        [JsonPropertyName("requestId")]
        public int? RequestId { get; set; }
        [JsonPropertyName("interventionId")]
        public int? InterventionId { get; set; }
        [JsonPropertyName("offerId")]
        public int? OfferId { get; set; }
        [JsonPropertyName("orderId")]
        public int? OrderId { get; set; }
        [JsonPropertyName("invoiceId")]
        public int? InvoiceId { get; set; }
    }
}