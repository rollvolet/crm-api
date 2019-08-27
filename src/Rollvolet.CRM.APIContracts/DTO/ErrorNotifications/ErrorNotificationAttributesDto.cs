using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.ErrorNotifications
{
    public class ErrorNotificationtAttributesDto
    {
        public string Version { get; set; }
        public string File { get; set; }
        [JsonProperty("line-number")]
        public int? LineNumber { get; set; }
        [JsonProperty("column-number")]
        public int? ColumnNumber { get; set; }
        [JsonProperty("current-url")]
        public string CurrentUrl { get; set; }
        [JsonProperty("current-path")]
        public string CurrentPath { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
    }
}