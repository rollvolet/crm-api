using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace Rollvolet.CRM.DataProvider.MsGraph.Authentication
{
    public class OnBehalfOfMsGraphAuthenticationProvider : IAuthenticationProvider
    {
        private static readonly string[] SCOPES = new string[] { "User.Read", "Calendars.ReadWrite.Shared" };
        
        private readonly IConfidentialClientApplication _clientApplication;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public OnBehalfOfMsGraphAuthenticationProvider(IConfidentialClientApplication clientApplication, IHttpContextAccessor httpContextAccessor,
            ILogger<OnBehalfOfMsGraphAuthenticationProvider> logger)
        {
            _clientApplication = clientApplication;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // see https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-v2-protocols-oauth-on-behalf-of
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            // Get the access token used to call this API
            string token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            _logger.LogDebug("Retrieved JWT token {token} from request authorization header", token);

            // We are passing an assertion to Azure AD about the current user. The assertion is a JWT Bearer token
            var userAssertion = new UserAssertion(token, "urn:ietf:params:oauth:grant-type:jwt-bearer");

            // Acquire access token on behalf of
            var result = await _clientApplication.AcquireTokenOnBehalfOfAsync(SCOPES, userAssertion);
            _logger.LogDebug("Acquired access token {token} to authorize against the Graph API", result.AccessToken);

            // Set the authorization header on the outgoing requests to Graph API
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
        }
    }
}