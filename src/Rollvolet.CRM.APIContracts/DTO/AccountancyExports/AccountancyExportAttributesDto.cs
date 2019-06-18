using System;
using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.AccountancyExports
{
    public class AccountancyExportAttributesDto
    {
        public DateTime Date { get; set; }
        [JsonProperty("from-number")]
        public int? FromNumber { get; set; }
        [JsonProperty("until-number")]
        public int? UntilNumber { get; set; }
        [JsonProperty("from-date")]
        public DateTime? FromDate { get; set; }
        [JsonProperty("until-date")]
        public DateTime? UntilDate { get; set; }
    }
}