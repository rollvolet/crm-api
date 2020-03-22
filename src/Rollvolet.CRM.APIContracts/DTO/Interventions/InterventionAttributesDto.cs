using System;
using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.Interventions
{
    public class InterventionAttributesDto
    {
        public DateTime? Date { get; set; }
        public string Comment { get; set; }
        [JsonProperty("cancellation-date")]
        public DateTime? CancellationDate { get; set; }
        [JsonProperty("cancellation-reason")]
        public string CancellationReason { get; set; }
    }
}