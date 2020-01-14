using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.Offerlines
{
    public class OfferlineAttributesDto
    {
        [JsonProperty("sequence-number")]
        public int SequenceNumber { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }
    }
}