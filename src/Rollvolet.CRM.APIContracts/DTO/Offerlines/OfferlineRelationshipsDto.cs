using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Offerlines
{
    public class OfferlineRelationshipsDto
    {
        public IRelationship Offer { get; set; }
        public IRelationship VatRate { get; set; }
    }
}