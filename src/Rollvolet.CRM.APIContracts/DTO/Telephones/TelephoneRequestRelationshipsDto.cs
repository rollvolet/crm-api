using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Telephones
{
    public class TelephoneRequestRelationshipsDto
    {
        public OneRelationship TelephoneType { get; set; }
        public OneRelationship Country { get; set; }
        public OneRelationship Customer { get; set; }
        public OneRelationship Contact { get; set; }
        public OneRelationship Building { get; set; }
    }
}