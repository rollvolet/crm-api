namespace Rollvolet.CRM.APIContracts.DTO.AuthenticationTokens
{
    public class AuthenticationTokenRequestDto
    {
        public string AuthorizationCode { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
    }
}