using System;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Customers
{
    public class CustomerRequestRelationshipsDto
    {
        public ManyRelationship Contacts { get; set; }
        public ManyRelationship Buildings { get; set; }
        public OneRelationship Country { get; set; }
        public OneRelationship Language { get; set; }
        [JsonProperty("honorific-prefix")]
        public OneRelationship HonorificPrefix { get; set; }
        public ManyRelationship Telephones { get; set; }
        public ManyRelationship Requests { get; set; }
        public ManyRelationship Interventions { get; set; }
        public ManyRelationship Offers { get; set; }
        public ManyRelationship Orders { get; set; }
        [JsonProperty("deposit-invoices")]
        public ManyRelationship DepositInvoices { get; set; }
        public ManyRelationship Invoices { get; set; }
        public ManyRelationship Tags { get; set; }
    }
}