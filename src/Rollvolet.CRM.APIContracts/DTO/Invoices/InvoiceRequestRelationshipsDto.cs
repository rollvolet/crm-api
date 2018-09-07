using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Invoices
{
    public class InvoiceRequestRelationshipsDto
    {
        public OneRelationship Order { get; set; }
        public OneRelationship Customer { get; set; }
        public OneRelationship Building { get; set; }
        public OneRelationship Contact { get; set; }
        [JsonProperty("vat-rate")]
        public OneRelationship VatRate { get; set; }
        public ManyRelationship Supplements { get; set; }
        public ManyRelationship Deposits { get; set; }
        [JsonProperty("deposit-invoices")]
        public ManyRelationship DepositInvoices { get; set; }
        [JsonProperty("working-hours")]
        public ManyRelationship WorkingHours { get; set; }
    }
}
