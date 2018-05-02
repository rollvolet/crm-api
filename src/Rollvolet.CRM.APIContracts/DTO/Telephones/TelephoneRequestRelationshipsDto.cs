using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Telephones
{
    public class TelephoneRequestRelationshipsDto
    {
        [JsonProperty("telephone-type")]
        public OneRelationship TelephoneType { get; set; }
        public OneRelationship Country { get; set; }
        public OneRelationship Customer { get; set; }
    }
}