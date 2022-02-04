using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Customers
{
    public class CustomerRequestRelationshipsDto
    {
        public ManyRelationship Contacts { get; set; }
        public ManyRelationship Buildings { get; set; }
        public OneRelationship Country { get; set; }
        public OneRelationship Language { get; set; }
        public OneRelationship HonorificPrefix { get; set; }
        public ManyRelationship Requests { get; set; }
        public ManyRelationship Interventions { get; set; }
        public ManyRelationship Offers { get; set; }
        public ManyRelationship Orders { get; set; }
        public ManyRelationship DepositInvoices { get; set; }
        public ManyRelationship Invoices { get; set; }
        public ManyRelationship Tags { get; set; }
    }
}