using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Customers
{
    public class CustomerRelationshipsDto
    {
        public IRelationship Contacts { get; set; }
        public IRelationship Buildings { get; set; }
        public IRelationship Country { get; set; }
        public IRelationship Language { get; set; }
        public IRelationship HonorificPrefix { get; set; }
        public IRelationship Telephones { get; set; }
        public IRelationship Requests { get; set; }
        public IRelationship Interventions { get; set; }
        public IRelationship Offers { get; set; }
        public IRelationship Orders { get; set; }
        public IRelationship DepositInvoices { get; set; }
        public IRelationship Invoices { get; set; }
        public IRelationship Tags { get; set; }
    }
}