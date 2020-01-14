using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.Invoicelines
{
    public class InvoicelineAttributesDto
    {
        [JsonProperty("sequence-number")]
        public int SequenceNumber { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }
    }
}