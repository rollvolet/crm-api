using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Telephones
{
    public class TelephoneRelationshipsDto
    {
        public IRelationship TelephoneType { get; set; }
        public IRelationship Country { get; set; }
        public IRelationship Customer { get; set; }
        public IRelationship Contact { get; set; }
        public IRelationship Building { get; set; }
    }
}