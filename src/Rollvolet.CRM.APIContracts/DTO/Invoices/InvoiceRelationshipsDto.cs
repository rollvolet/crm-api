using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Invoices
{
    public class InvoiceRelationshipsDto
    {
        public IRelationship Intervention { get; set; }
        public IRelationship Order { get; set; }
        public IRelationship Customer { get; set; }
        public IRelationship Building { get; set; }
        public IRelationship Contact { get; set; }
        public IRelationship VatRate { get; set; }
        public IRelationship Supplements { get; set; }
        public IRelationship Invoicelines { get; set; }
        public IRelationship Deposits { get; set; }
        public IRelationship DepositInvoices { get; set; }
        public IRelationship WorkingHours { get; set; }
    }
}
