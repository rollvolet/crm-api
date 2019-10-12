using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.ProductUnits
{
    public class ProductUnitAttributesDto
    {
        public string Code { get; set; }
        [JsonProperty("name-ned")]
        public string NameNed { get; set; }
        [JsonProperty("name-fra")]
        public string NameFra { get; set; }
    }
}