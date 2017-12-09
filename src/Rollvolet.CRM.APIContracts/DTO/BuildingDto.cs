using System;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class BuildingDto : Resource<BuildingDto.AttributesDto, BuildingDto.RelationshipsDto>
    {        
        public class AttributesDto {
            public string Name { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Address3 { get; set; }
            [JsonProperty("postal-code")]
            public string PostalCode { get; set; }
            public string City { get; set; }
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
            public int Number { get; set; }
            [JsonProperty("customer-id")]
            public int CustomerId { get; set; }
            public DateTime Created { get; set; }
            public DateTime Updated { get; set; }
        }

        public class RelationshipsDto
        {
            public IRelationship Country { get; set; }
            public IRelationship Language { get; set; }
            [JsonProperty("honorific-prefix")]
            public IRelationship HonorificPrefix { get; set; }
            public IRelationship Telephones { get; set; } 
        }
  }
}