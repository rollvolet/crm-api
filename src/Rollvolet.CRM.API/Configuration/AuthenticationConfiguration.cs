namespace Rollvolet.CRM.API.Configuration
{
    public class AuthenticationConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseUri { get; set; }
        public string TenantId { get; set; }
    }
}