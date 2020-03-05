using System;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Orders
{
    public class OrderRequestRelationshipsDto
    {
        public OneRelationship Offer { get; set; }
        public OneRelationship Invoice { get; set; }
        public OneRelationship Customer { get; set; }
        public OneRelationship Building { get; set; }
        public OneRelationship Contact { get; set; }
        [JsonProperty("vat-rate")]
        public OneRelationship VatRate { get; set; }
        public ManyRelationship Invoicelines { get; set; }
        public ManyRelationship Deposits { get; set; }
        [JsonProperty("deposit-invoices")]
        public ManyRelationship DepositInvoices { get; set; }
        public ManyRelationship Interventions { get; set; }
    }
}