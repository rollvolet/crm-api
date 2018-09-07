using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Invoices
{
    public class InvoiceRelationshipsDto
    {
        public IRelationship Order { get; set; }
        public IRelationship Customer { get; set; }
        public IRelationship Building { get; set; }
        public IRelationship Contact { get; set; }
        [JsonProperty("vat-rate")]
        public IRelationship VatRate { get; set; }
        public IRelationship Supplements { get; set; }
        public IRelationship Deposits { get; set; }
        [JsonProperty("deposit-invoices")]
        public IRelationship DepositInvoices { get; set; }
        [JsonProperty("working-hours")]
        public IRelationship WorkingHours { get; set; }
    }
}
