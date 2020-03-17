using System;
using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.PlanningEvents
{
    public class PlanningEventAttributesDto
    {
        public DateTime? Date { get; set; }
        [JsonProperty("ms-object-id")]
        public string MsObjectId { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("period")]
        public string Period { get; set; }
        [JsonProperty("from-hour")]
        public string FromHour { get; set; }
        [JsonProperty("until-hour")]
        public string UntilHour { get; set; }
        [JsonProperty("is-not-available-in-calendar")]
        public bool IsNotAvailableInCalendar { get; set; }
    }
}