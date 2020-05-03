using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Invoicelines
{
    public class InvoicelineRelationshipsDto
    {
        public IRelationship Order { get; set; }
        public IRelationship Invoice { get; set; }
        public IRelationship VatRate { get; set; }
    }
}