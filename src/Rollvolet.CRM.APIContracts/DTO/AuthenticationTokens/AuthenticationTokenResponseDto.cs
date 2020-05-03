using System.Text.Json.Serialization;

namespace Rollvolet.CRM.APIContracts.DTO.AuthenticationTokens
{
    public class AuthenticationTokenResponseDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        public string Scope { get; set; }
    }
}