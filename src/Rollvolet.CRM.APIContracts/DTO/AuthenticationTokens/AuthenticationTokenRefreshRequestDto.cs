namespace Rollvolet.CRM.APIContracts.DTO.AuthenticationTokens
{
    public class AuthenticationTokenRefreshRequestDto
    {
        public string RefreshToken { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
    }
}