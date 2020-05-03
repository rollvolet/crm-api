using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.WorkingHours
{
    public class WorkingHourRelationshipsDto
    {
        public IRelationship Invoice { get; set; }
        public IRelationship Employee { get; set; }
    }
}