using System;
using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.Offers
{
    public class OfferAttributesDto
    {
        public string Number { get; set; }
        [JsonProperty("sequence-number")]
        public int SequenceNumber { get; set; }
        [JsonProperty("request-number")]
        public int? RequestNumber { get; set; }
        [JsonProperty("offer-date")]
        public DateTime OfferDate { get; set; }
        public string Comment { get; set; }
        public string Reference { get; set; }
        [JsonProperty("document-intro")]
        public string DocumentIntro { get; set; }
        [JsonProperty("document-outro")]
        public string DocumentOutro { get; set; }
        [JsonProperty("document-version")]
        public string DocumentVersion { get; set; }
    }
}