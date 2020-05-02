using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace Rollvolet.CRM.DataProvider.MsGraph.Authentication
{
    public class OnBehalfOfMsGraphAuthenticationProvider : IAuthenticationProvider
    {
        private static readonly string[] SCOPES = new string[] { "User.Read", "Calendars.ReadWrite.Shared" };

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly ILogger _logger;

        public OnBehalfOfMsGraphAuthenticationProvider(IHttpContextAccessor httpContextAccessor, ITokenAcquisition tokenAcquisition, ILogger<OnBehalfOfMsGraphAuthenticationProvider> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _tokenAcquisition = tokenAcquisition;
            _logger = logger;
        }

        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            _logger.LogInformation("Exchanging access token for token to query Graph API");
            string accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(SCOPES);

            // Append the access token to the request
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}