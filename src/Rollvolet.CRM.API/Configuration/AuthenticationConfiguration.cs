namespace Rollvolet.CRM.API.Configuration
{
    public class AuthenticationConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Instance { get; set; }
        public string TenantId { get; set; }

        public string Authority {
            get {
                return $"{Instance}/{TenantId}/v2.0";
            }
        }
    }
}