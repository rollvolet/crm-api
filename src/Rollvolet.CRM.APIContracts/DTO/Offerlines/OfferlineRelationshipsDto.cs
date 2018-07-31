using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Offerlines
{
    public class OfferlineRelationshipsDto
    {
        public IRelationship Offer { get; set; }
        [JsonProperty("vat-rate")]
        public IRelationship VatRate { get; set; }
    }
}