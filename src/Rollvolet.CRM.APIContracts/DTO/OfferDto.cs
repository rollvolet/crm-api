using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class OfferDto : Resource<OfferDto.AttributesDto, OfferDto.RelationshipsDto>
    {
        public class AttributesDto
        {
            public string Number { get; set; }        
            [JsonProperty("sequence-number")]         
            public int SequenceNumber { get; set; }        
            [JsonProperty("offer-date")]         
            public DateTime OfferDate { get; set; }
            public double Amount { get; set; }        
            [JsonProperty("submission-date")]         
            public DateTime SubmissionDate { get; set; }        
            [JsonProperty("foreseen-hours")]         
            public double ForeseenHours { get; set; }        
            [JsonProperty("foreseen-nb-of-persons")]
            public double ForeseenNbOfPersons { get; set; }
            public string Comment { get; set; } 
            public string Reference { get; set; }
            public bool Canceled { get; set; }
            public DateTime Updated { get; set; }
        }

        public class RelationshipsDto
        {
            public IRelationship Request { get; set; }
            public IRelationship Order { get; set; }
            public IRelationship Customer { get; set; }
            public IRelationship Building { get; set; }
            public IRelationship Contact { get; set; }
            [JsonProperty("submission-type")]
            public IRelationship SubmissionType { get; set; }
            [JsonProperty("vat-rate")]
            public IRelationship VatRate { get; set; }
            public IRelationship Product { get; set; }
        }
    }
}