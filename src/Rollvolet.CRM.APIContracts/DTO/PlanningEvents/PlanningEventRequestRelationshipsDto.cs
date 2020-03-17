using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.PlanningEvents
{
    public class PlanningEventRequestRelationshipsDto
    {
        public OneRelationship Intervention { get; set; }
        public OneRelationship Order { get; set; }
    }
}