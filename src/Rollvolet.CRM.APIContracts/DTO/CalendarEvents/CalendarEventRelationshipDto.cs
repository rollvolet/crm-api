using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.CalendarEvents
{
    public class CalendarEventRelationshipsDto
    {
        public IRelationship Customer { get; set; }
        public IRelationship Request { get; set; }
    }
}