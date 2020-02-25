using System;
using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.Interventions
{
    public class InterventionAttributesDto
    {
        public DateTime? Date { get; set; }
        public string Comment { get; set; }
        [JsonProperty("has-production-ticket")]
        public bool HasProductionTicket { get; set; }
        [JsonProperty("planning-date")]
        public DateTime? PlanningDate { get; set; }
        [JsonProperty("planning-ms-object-id")]
        public string PlanningMsObjectId { get; set; }
    }
}