using System;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Visits
{
    public class VisitAttributesDto
    {
        [JsonProperty("offer-expected")]
        public bool OfferExpected { get; set; }
        public string Visitor { get; set; }
        [JsonProperty("visit-date")]
        public DateTime? VisitDate { get; set; }
        public string Period { get; set; }
        [JsonProperty("from-hour")]
        public string FromHour { get; set; }
        [JsonProperty("until-hour")]
        public string UntilHour { get; set; }
        public string Comment { get; set; }
        [JsonProperty("calendar-subject")]
        public string CalendarSubject { get; set; }
        [JsonProperty("calendar-id")]
        public string CalendarId { get; set; }
        [JsonProperty("ms-object-id")]
        public string MsObjectId { get; set; }
    }
}