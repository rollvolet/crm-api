using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Requests
{
    public class RequestRequestRelationshipsDto
    {
        public OneRelationship Customer { get; set; }
        public OneRelationship Building { get; set; }
        public OneRelationship Contact { get; set; }
        public OneRelationship WayOfEntry { get; set; }
        public OneRelationship CalendarEvent { get; set; }
        public OneRelationship Offer { get; set; }
        public OneRelationship Origin { get; set; }
    }
}