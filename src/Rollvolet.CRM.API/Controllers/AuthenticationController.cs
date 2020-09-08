using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.APIContracts.DTO.AuthenticationTokens;
using System.Net.Http;
using System.Text.Json;
using Rollvolet.CRM.APIContracts.JsonApi;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Rollvolet.CRM.Domain.Authentication.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.Identity.Client;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Authorize]
    public class AuthenticationController : ControllerBase
    {
        // TODO remove duplication
        private static readonly string[] SCOPES = new string[] { "User.Read", "Calendars.ReadWrite.Shared", "Files.ReadWrite.All" };

        private readonly IConfidentialClientApplicationProvider _applicationProvider;
        private readonly ILogger _logger;
        private HttpClient _httpClient;

        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions {
            PropertyNamingPolicy = new JsonApiNamingPolicy()
        };

        public AuthenticationController(IConfidentialClientApplicationProvider applicationProvider,
            Narato.Correlations.Http.Interfaces.IHttpClientFactory httpClientFactory,
            ILogger<AuthenticationController> logger)
        {
            _applicationProvider = applicationProvider;
            _httpClient = httpClientFactory.Create();
            _logger = logger;
        }

        [HttpPost("sessions")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto requestDto)
        {
            var scopes = requestDto.Scope.Split(",");
            var authenticationResult = await _applicationProvider.GetApplication()
                                        .AcquireTokenByAuthorizationCode(scopes, requestDto.AuthorizationCode)
                                        .ExecuteAsync();
            _logger.LogDebug("Exchanged authorization code for access token: {0}", authenticationResult.AccessToken);
            var sessionInfo = GetSessionInfo(authenticationResult);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Upn, sessionInfo.Account),
                new Claim(ClaimTypes.Name, sessionInfo.Name),
                new Claim(ClaimTypes.Role, sessionInfo.Roles)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var user = new ClaimsPrincipal(claimsIdentity);
            var authenticationProperties = new AuthenticationProperties { };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user, authenticationProperties);

            return Ok(sessionInfo);
        }

        [HttpGet("sessions/current")]
        public async Task<IActionResult> GetCurrentSession()
        {
            var username = HttpContext.User.FindFirstValue(ClaimTypes.Upn);
            // make sure TokenCache still contains a token
            await _applicationProvider.GetApplication().AcquireTokenSilent(SCOPES, username).ExecuteAsync();

            return Ok(new SessionInfoDto {
                Account = HttpContext.User.FindFirstValue(ClaimTypes.Upn),
                Name = HttpContext.User.FindFirstValue(ClaimTypes.Name),
                Roles = HttpContext.User.FindFirstValue(ClaimTypes.Role)
            });
        }

        [HttpDelete("sessions/current")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return NoContent();
        }

        private SessionInfoDto GetSessionInfo(AuthenticationResult authenticationResult)
        {
            var jwt = (JwtSecurityToken) new JwtSecurityTokenHandler().ReadToken(authenticationResult.AccessToken);
            _logger.LogDebug("Exchanged authorization code for access token: {0}", authenticationResult.AccessToken);
            var name = jwt.Claims.FirstOrDefault(claim => claim.Type == "name");
            var roles = jwt.Claims.FirstOrDefault(claim => claim.Type == "roles");
            return new SessionInfoDto {
                Account = authenticationResult.Account.Username,
                Name = name != null ? name.Value : "",
                Roles = roles != null ? roles.Value : ""
            };
        }
    }
}