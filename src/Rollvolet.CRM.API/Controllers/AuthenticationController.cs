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
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Identity.Web;
using Rollvolet.CRM.Domain.Authentication.Interfaces;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
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

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AuthenticationTokenRequestDto requestDto)
        {
            var scopes = requestDto.Scope.Split(",");
            var authenticationResult = await _applicationProvider.GetApplication()
                                        .AcquireTokenByAuthorizationCode(scopes, requestDto.AuthorizationCode)
                                        .ExecuteAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Upn, authenticationResult.Account.Username),
                new Claim(ClaimConstants.Scope, String.Join(",", authenticationResult.Scopes))
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var user = new ClaimsPrincipal(claimsIdentity);
            var authenticationProperties = new AuthenticationProperties
            {
                ExpiresUtc = authenticationResult.ExpiresOn
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user, authenticationProperties);

            var expiresIn = (authenticationResult.ExpiresOn - DateTimeOffset.Now).TotalSeconds;
            return Ok(new AuthenticationTokenResponseDto {
                TokenType = "Bearer",
                AccessToken = authenticationResult.AccessToken,
                ExpiresIn = Convert.ToInt32(expiresIn),
                Scope = String.Join(',', authenticationResult.Scopes)
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return NoContent();
        }
    }
}