namespace Rollvolet.CRM.API.Configuration
{
    public class AuthenticationConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseUri { get; set; }
        public string TenantId { get; set; }
        public string RedirectUri { get; set; }

        public string Authority {
            get {
                return $"{BaseUri}/{TenantId}/v2.0";
            }
        }
    }
}