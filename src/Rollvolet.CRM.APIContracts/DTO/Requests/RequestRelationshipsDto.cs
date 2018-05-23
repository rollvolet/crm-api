using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Requests
{
    public class RequestRelationshipsDto
    {
        public IRelationship Customer { get; set; }
        public IRelationship Building { get; set; }
        public IRelationship Contact { get; set; }
        [JsonProperty("way-of-entry")]
        public IRelationship WayOfEntry { get; set; }
        public IRelationship Visit { get; set; }
        public IRelationship Offer { get; set; }
    }
}