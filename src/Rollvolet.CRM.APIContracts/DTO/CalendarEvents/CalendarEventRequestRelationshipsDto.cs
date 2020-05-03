using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.CalendarEvents
{
    public class CalendarEventRequestRelationshipsDto
    {
        public OneRelationship Customer { get; set; }
        public OneRelationship Request { get; set; }
    }
}