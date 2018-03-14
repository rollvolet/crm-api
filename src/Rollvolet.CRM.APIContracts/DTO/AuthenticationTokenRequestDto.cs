namespace Rollvolet.CRM.APIContracts.DTO
{
    public class AuthenticationTokenRequestDto
    {   
        public string AuthorizationCode { get; set; }
        public string RedirectUri { get; set; }
    }
}