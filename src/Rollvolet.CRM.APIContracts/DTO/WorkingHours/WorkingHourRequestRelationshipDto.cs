using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.WorkingHours
{
    public class WorkingHourRequestRelationshipsDto
    {
        public OneRelationship Invoice { get; set; }
        public OneRelationship Employee { get; set; }
    }
}