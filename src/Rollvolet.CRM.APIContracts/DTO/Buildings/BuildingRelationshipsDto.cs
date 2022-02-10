using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Buildings
{
    public class BuildingRelationshipsDto
    {
        public IRelationship Country { get; set; }
        public IRelationship Language { get; set; }
        public IRelationship HonorificPrefix { get; set; }
        public IRelationship Customer { get; set; }
    }
}