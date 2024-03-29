using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Interventions
{
    public class InterventionRelationshipsRequestDto
    {
        public OneRelationship FollowUpRequest { get; set; }
        public OneRelationship Invoice { get; set; }
        public OneRelationship Origin { get; set; }
        public OneRelationship Customer { get; set; }
        public OneRelationship Building { get; set; }
        public OneRelationship Contact { get; set; }
        public OneRelationship WayOfEntry { get; set; }
        public OneRelationship Employee { get; set; }
        public ManyRelationship Technicians { get; set; }
    }
}