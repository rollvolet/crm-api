using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO.Offers
{
    public class OfferRequestRelationshipsDto
    {
        public OneRelationship Request { get; set; }
        public OneRelationship Order { get; set; }
        public OneRelationship Customer { get; set; }
        public OneRelationship Building { get; set; }
        public OneRelationship Contact { get; set; }
        [JsonProperty("submission-type")]
        public OneRelationship SubmissionType { get; set; }
        [JsonProperty("vat-rate")]
        public OneRelationship VatRate { get; set; }
        public ManyRelationship Offerlines { get; set; }
    }
}