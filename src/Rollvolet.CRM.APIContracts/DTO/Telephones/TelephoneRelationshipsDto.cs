using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Telephones
{
    public class TelephoneRelationshipsDto
    {
        [JsonProperty("telephone-type")]
        public IRelationship TelephoneType { get; set; }
        public IRelationship Country { get; set; }
        public IRelationship Customer { get; set; }
    }
}