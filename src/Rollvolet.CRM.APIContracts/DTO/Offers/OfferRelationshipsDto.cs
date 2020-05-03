using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Offers
{
    public class OfferRelationshipsDto
    {
        public IRelationship Request { get; set; }
        public IRelationship Order { get; set; }
        public IRelationship Customer { get; set; }
        public IRelationship Building { get; set; }
        public IRelationship Contact { get; set; }
        public IRelationship VatRate { get; set; }
        public IRelationship Offerlines { get; set; }
    }
}