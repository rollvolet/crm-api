using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class DepositDto : Resource<DepositDto.AttributesDto, DepositDto.RelationshipsDto>
    {
        public class AttributesDto
        {
            [JsonProperty("sequence-number")]         
            public int SequenceNumber { get; set; }
            public double? Amount { get; set; }   
            [JsonProperty("payment-date")]     
            public DateTime PaymentDate { get; set; }
        }

        public class RelationshipsDto
        {
            public IRelationship Customer { get; set; }
            public IRelationship Order { get; set; }
            public IRelationship Payment { get; set; }
        }
    }
}