using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Invoicelines
{
    public class InvoicelineRequestRelationshipsDto
    {
        public OneRelationship Order { get; set; }
        public OneRelationship Invoice { get; set; }
        [JsonProperty("vat-rate")]
        public OneRelationship VatRate { get; set; }
    }
}