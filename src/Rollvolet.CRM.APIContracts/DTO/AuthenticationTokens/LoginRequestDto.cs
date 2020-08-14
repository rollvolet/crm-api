namespace Rollvolet.CRM.APIContracts.DTO.AuthenticationTokens
{
    public class LoginRequestDto
    {
        public string AuthorizationCode { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
    }
}