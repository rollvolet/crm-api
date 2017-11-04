using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class CustomerDto : Resource<CustomerDto.AttributesDto, CustomerDto.RelationshipsDto>
    {
        public class AttributesDto
        {
            [JsonProperty("data-id")]
            public int DataId { get; set; }
            public int Number { get; set; }
            public string Name { get; set; }
            [JsonProperty("address-1")]
            public string Address1 { get; set; }
            [JsonProperty("address-2")]
            public string Address2 { get; set; }
            [JsonProperty("address-3")]
            public string Address3 { get; set; }
            [JsonProperty("is-company")]
            public bool IsCompany { get; set; }
            [JsonProperty("vat-number")]
            public string VatNumber { get; set; }
            public string Prefix { get; set; }
            public string Suffix { get; set; }
            public string Email { get; set; }
            [JsonProperty("email-2")]
            public string Email2 { get; set; }
            public string Url { get; set; }
            [JsonProperty("print-prefix")]
            public bool PrintPrefix { get; set; }
            [JsonProperty("print-suffix")]
            public bool PrintSuffix { get; set; }
            [JsonProperty("print-in-front")]
            public bool PrintInFront { get; set; }
            public string Comment { get; set; }
            public DateTime Created { get; set; }
            public DateTime Updated { get; set; }
        }

        public class RelationshipsDto
        {
            public ManyRelationship Contacts { get; set; }
            public ManyRelationship Buildings { get; set; }
            public OneRelationship Country { get; set; }
            public OneRelationship Language { get; set; }
            [JsonProperty("postal-code")]
            public OneRelationship PostalCode { get; set; }
            [JsonProperty("honorific-prefix")]
            public OneRelationship HonorificPrefix { get; set; }
            public ManyRelationship Telephones { get; set; } 
        }
    }
}