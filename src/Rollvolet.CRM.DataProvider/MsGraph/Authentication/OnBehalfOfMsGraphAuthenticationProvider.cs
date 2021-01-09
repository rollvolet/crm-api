using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Rollvolet.CRM.Domain.Authentication.Interfaces;

namespace Rollvolet.CRM.DataProvider.MsGraph.Authentication
{
    public class OnBehalfOfMsGraphAuthenticationProvider : IAuthenticationProvider
    {
        // TODO remove duplication
        private static readonly string[] SCOPES = new string[] { "User.Read", "Calendars.ReadWrite.Shared", "Files.ReadWrite.All" };

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfidentialClientApplicationProvider _applicationProvider;
        private readonly ILogger _logger;

        public OnBehalfOfMsGraphAuthenticationProvider(IHttpContextAccessor httpContextAccessor,
            IConfidentialClientApplicationProvider applicationProvider, ILogger<OnBehalfOfMsGraphAuthenticationProvider> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _applicationProvider = applicationProvider;
            _logger = logger;
        }

        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            // TODO query access token from triplestore based on mu-session header
            var user = _httpContextAccessor.HttpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                var username = user.FindFirstValue(ClaimTypes.Upn);

                _logger.LogInformation("Query token cache to get access token to query Graph API");
                var authenticationResult = await _applicationProvider.GetApplication().AcquireTokenSilent(SCOPES, username).ExecuteAsync();

                // Append the access token to the request
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
            }
            else
            {
                throw new AuthenticationException();
            }
        }
    }
}