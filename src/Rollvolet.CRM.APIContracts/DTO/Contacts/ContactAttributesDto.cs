using System;
using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.Contacts
{
    public class ContactAttributesDto
    { 
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        [JsonProperty("postal-code")]
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Url { get; set; }
        [JsonProperty("print-prefix")]
        public bool PrintPrefix { get; set; }
        [JsonProperty("print-suffix")]
        public bool PrintSuffix { get; set; }
        [JsonProperty("print-in-front")]
        public bool PrintInFront { get; set; }
        public string Comment { get; set; }
        public int Number { get; set; }
        public DateTime Created { get; set; }
    }
}