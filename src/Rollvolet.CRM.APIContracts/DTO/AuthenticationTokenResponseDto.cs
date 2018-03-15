using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class AuthenticationTokenResponseDto
    {   
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        public string Scope { get; set; }
    }
}