using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class RequestDto : Resource<RequestDto.AttributesDto, RequestDto.RelationshipsDto>
    {
        public class AttributesDto
        {
            [JsonProperty("request-date")]          
            public DateTime RequestDate { get; set; }
            [JsonProperty("requires-visit")]
            public bool RequiresVisit { get; set; }
            public string Comment { get; set; }
            public string Employee { get; set; }
            public DateTime Updated { get; set; }
        }

        public class RelationshipsDto
        {
            public IRelationship Customer { get; set; }
            public IRelationship Building { get; set; }
            public IRelationship Contact { get; set; }
            [JsonProperty("way-of-entry")]
            public IRelationship WayOfEntry { get; set; }
            public IRelationship Visit { get; set; }
        }
    }
}