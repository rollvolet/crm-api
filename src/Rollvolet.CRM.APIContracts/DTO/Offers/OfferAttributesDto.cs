using System;
using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.Offers
{
    public class OfferAttributesDto
    {
        public string Number { get; set; }
        [JsonProperty("sequence-number")]
        public int SequenceNumber { get; set; }
        [JsonProperty("offer-date")]
        public DateTime OfferDate { get; set; }
        public double? Amount { get; set; }
        [JsonProperty("submission-date")]
        public DateTime? SubmissionDate { get; set; }
        [JsonProperty("foreseen-hours")]
        public double? ForeseenHours { get; set; }
        [JsonProperty("foreseen-nb-of-persons")]
        public double? ForeseenNbOfPersons { get; set; }
        public string Comment { get; set; }
        public string Reference { get; set; }
    }
}