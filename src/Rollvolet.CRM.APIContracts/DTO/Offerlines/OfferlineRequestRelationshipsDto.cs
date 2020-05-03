using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Offerlines
{
    public class OfferlineRequestRelationshipsDto
    {
        public OneRelationship Offer { get; set; }
        public OneRelationship VatRate { get; set; }
    }
}