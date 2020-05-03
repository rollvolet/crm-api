using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.DepositInvoices
{
    public class DepositInvoiceRelationshipsRequestDto
    {
        public OneRelationship Order { get; set; }
        public OneRelationship Customer { get; set; }
        public OneRelationship Building { get; set; }
        public OneRelationship Contact { get; set; }
        public OneRelationship VatRate { get; set; }
    }
}
