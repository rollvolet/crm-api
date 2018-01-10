namespace Rollvolet.CRM.APIContracts.DTO
{
    public class AuthenticationRequestDto
    {   
        public string AuthorizationCode { get; set; }
        public string RedirectUri { get; set; }
    }
}