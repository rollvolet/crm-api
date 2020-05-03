using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Requests
{
    public class RequestRelationshipsDto
    {
        public IRelationship Customer { get; set; }
        public IRelationship Building { get; set; }
        public IRelationship Contact { get; set; }
        public IRelationship WayOfEntry { get; set; }
        public IRelationship CalendarEvent { get; set; }
        public IRelationship Offer { get; set; }
        public IRelationship Origin { get; set; }
    }
}