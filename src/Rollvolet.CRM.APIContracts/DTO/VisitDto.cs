using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class VisitDto : Resource<VisitDto.AttributesDto, VisitDto.RelationshipsDto>
    {
        public class AttributesDto
        {
            [JsonProperty("offer-expected")]
            public bool OfferExpected { get; set; }
            public string Visitor { get; set; }
            [JsonProperty("visit-date")]
            public DateTime? VisitDate { get; set; }
            public string Comment { get; set; }
            [JsonProperty("calendar-subject")]            
            public string CalendarSubject { get; set; }
            [JsonProperty("calendar-id")]            
            public string CalendarId { get; set; }          
        }

        public class RelationshipsDto
        {
            public IRelationship Customer { get; set; }
            public IRelationship Request { get; set; }
        }
    }
}