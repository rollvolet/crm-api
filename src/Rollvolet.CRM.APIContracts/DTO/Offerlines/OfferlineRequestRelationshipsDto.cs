using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Offerlines
{
    public class OfferlineRequestRelationshipsDto
    {
        public OneRelationship Offer { get; set; }
        [JsonProperty("vat-rate")]
        public OneRelationship VatRate { get; set; }
    }
}