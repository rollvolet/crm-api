using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Buildings
{
    public class BuildingRequestRelationshipsDto
    {
        public OneRelationship Country { get; set; }
        public OneRelationship Language { get; set; }
        [JsonProperty("honorific-prefix")]
        public OneRelationship HonorificPrefix { get; set; }
        public ManyRelationship Telephones { get; set; }
        public OneRelationship Customer { get; set; }
        public ManyRelationship Requests { get; set; }
        public ManyRelationship Invoices { get; set; }
    }
}