using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Buildings
{
    public class BuildingRequestRelationshipsDto
    {
        public OneRelationship Country { get; set; }
        public OneRelationship Language { get; set; }
        public OneRelationship HonorificPrefix { get; set; }
        public OneRelationship Customer { get; set; }
    }
}