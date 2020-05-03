using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Deposits
{
    public class DepositRelationshipsDto
    {
        public IRelationship Customer { get; set; }
        public IRelationship Order { get; set; }
        public IRelationship Invoice { get; set; }
        public IRelationship Payment { get; set; }
    }
}