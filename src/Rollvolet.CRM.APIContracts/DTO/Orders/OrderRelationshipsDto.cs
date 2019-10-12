using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Orders
{
    public class OrderRelationshipsDto
    {
        public IRelationship Offer { get; set; }
        public IRelationship Invoice { get; set; }
        public IRelationship Customer { get; set; }
        public IRelationship Building { get; set; }
        public IRelationship Contact { get; set; }
        [JsonProperty("vat-rate")]
        public IRelationship VatRate { get; set; }
        public IRelationship Deposits { get; set; }
        [JsonProperty("deposit-invoices")]
        public IRelationship DepositInvoices { get; set; }
    }
}