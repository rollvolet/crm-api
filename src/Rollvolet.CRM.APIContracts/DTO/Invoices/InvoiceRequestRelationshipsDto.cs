using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Invoices
{
    public class InvoiceRequestRelationshipsDto
    {
        public OneRelationship Intervention { get; set; }
        public OneRelationship Order { get; set; }
        public OneRelationship Customer { get; set; }
        public OneRelationship Building { get; set; }
        public OneRelationship Contact { get; set; }
        public OneRelationship VatRate { get; set; }
        public ManyRelationship Supplements { get; set; }
        public ManyRelationship Invoicelines { get; set; }
        public ManyRelationship Deposits { get; set; }
        public ManyRelationship DepositInvoices { get; set; }
        public ManyRelationship WorkingHours { get; set; }
    }
}
