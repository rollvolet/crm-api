using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class InvoiceSupplementDto : Resource<InvoiceSupplementDto.AttributesDto, EmptyRelationshipsDto>
    {
        public class AttributesDto
        {  
            public int Id { get; set; }  
            [JsonProperty("sequence-number")]
            public int SequenceNumber { get; set; }   
            [JsonProperty("nb-of-pieces")]
            public double? NbOfPieces { get; set; } 
            public double? Amount { get; set; } 
            public string Description { get; set; }
        }
    }
}