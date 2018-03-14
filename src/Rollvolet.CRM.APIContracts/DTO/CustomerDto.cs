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
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Address3 { get; set; }
            [JsonProperty("postal-code")]
            public string PostalCode { get; set; }
            public string City { get; set; }
            [JsonProperty("is-company")]
            public bool IsCompany { get; set; }
            [JsonProperty("vat-number")]
            public string VatNumber { get; set; }
            public string Prefix { get; set; }
            public string Suffix { get; set; }
            public string Email { get; set; }
            public string Email2 { get; set; }
            public string Url { get; set; }
            [JsonProperty("print-prefix")]
            public bool PrintPrefix { get; set; }
            [JsonProperty("print-suffix")]
            public bool PrintSuffix { get; set; }
            [JsonProperty("print-in-front")]
            public bool PrintInFront { get; set; }
            public string Comment { get; set; }
            public string Memo { get; set; }
            public DateTime Created { get; set; }
        }

        public class RelationshipsDto
        {
            public IRelationship Contacts { get; set; }
            public IRelationship Buildings { get; set; }
            public IRelationship Country { get; set; }
            public IRelationship Language { get; set; }
            [JsonProperty("honorific-prefix")]
            public IRelationship HonorificPrefix { get; set; }
            public IRelationship Telephones { get; set; }
            public IRelationship Requests { get; set; }
            public IRelationship Offers { get; set; }
            public IRelationship Orders { get; set; }
            public IRelationship Invoices { get; set; }            
            public IRelationship Tags { get; set; }
        }
    }
}