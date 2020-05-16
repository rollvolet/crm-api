using System;
using System.Text.Json.Serialization;

namespace Rollvolet.CRM.APIContracts.DTO.Buildings
{
    public class BuildingAttributesDto
    {
        public string Name { get; set; }
        [JsonPropertyName("address1")]
        public string Address1 { get; set; }
        [JsonPropertyName("address2")]
        public string Address2 { get; set; }
        [JsonPropertyName("address3")]
        public string Address3 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string Email { get; set; }
        [JsonPropertyName("email2")]
        public string Email2 { get; set; }
        public string Url { get; set; }
        public bool PrintPrefix { get; set; }
        public bool PrintSuffix { get; set; }
        public bool PrintInFront { get; set; }
        public string Comment { get; set; }
        public int Number { get; set; }
        public DateTime Created { get; set; }
    }
}