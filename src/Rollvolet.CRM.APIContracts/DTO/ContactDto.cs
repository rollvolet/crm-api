using System;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class ContactDto : Resource<ContactDto.AttributesDto, ContactDto.RelationshipsDto>
    {
        public class AttributesDto {
            public string Name { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Address3 { get; set; }
            public int Number { get; set; }
            [JsonProperty("customer-id")]
            public int CustomerId { get; set; }
            public DateTime Created { get; set; }
            public DateTime Updated { get; set; }
        }

        public class RelationshipsDto
        {
            public OneRelationship Customer { get; set; }
            public OneRelationship Country { get; set; }
            public OneRelationship Language { get; set; }
            [JsonProperty("postal-code")]
            public OneRelationship PostalCode { get; set; }
        }
  }
}