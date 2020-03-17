using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.PlanningEvents
{
    public class PlanningEventRelationshipsDto
    {
        public IRelationship Intervention { get; set; }
        public IRelationship Order { get; set; }
    }
}