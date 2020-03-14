using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Interventions
{
    public class InterventionRelationshipsDto
    {
        [JsonProperty("follow-up-request")]
        public IRelationship FollowUpRequest { get; set; }
        public IRelationship Invoice { get; set; }
        public IRelationship Origin { get; set; }
        public IRelationship Customer { get; set; }
        public IRelationship Building { get; set; }
        public IRelationship Contact { get; set; }
        [JsonProperty("way-of-entry")]
        public IRelationship WayOfEntry { get; set; }
        public IRelationship Employee { get; set; }
        public IRelationship Technicians { get; set; }
    }
}