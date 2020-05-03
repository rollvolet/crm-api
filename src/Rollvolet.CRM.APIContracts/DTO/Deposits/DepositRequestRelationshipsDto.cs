using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Deposits
{
    public class DepositRequestRelationshipsDto
    {
        public OneRelationship Customer { get; set; }
        public OneRelationship Order { get; set; }
        public OneRelationship Invoice { get; set; }
        public OneRelationship Payment { get; set; }
    }
}