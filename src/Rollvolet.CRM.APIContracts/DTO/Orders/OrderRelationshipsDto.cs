using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Orders
{
    public class OrderRelationshipsDto
    {
        public IRelationship Offer { get; set; }
        public IRelationship Invoice { get; set; }
        public IRelationship Customer { get; set; }
        public IRelationship Building { get; set; }
        public IRelationship Contact { get; set; }
        public IRelationship VatRate { get; set; }
        public IRelationship Deposits { get; set; }
        public IRelationship DepositInvoices { get; set; }
        public IRelationship Interventions { get; set; }
        public IRelationship Technicians { get; set; }
    }
}