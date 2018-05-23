using System;
using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.Requests
{
    public class RequestAttributesDto
    {
        [JsonProperty("request-date")]
        public DateTime RequestDate { get; set; }
        [JsonProperty("requires-visit")]
        public bool RequiresVisit { get; set; }
        public string Comment { get; set; }
        public string Employee { get; set; }
    }
}