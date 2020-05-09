using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rollvolet.CRM.APIContracts.DTO.AuthenticationTokens;
using Rollvolet.CRM.API.Configuration;
using System.Net.Http;
using System.Text.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private AuthenticationConfiguration _authenticationConfiguration;
        private readonly ILogger _logger;
        private HttpClient _httpClient;

        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions {
            PropertyNamingPolicy = new JsonApiNamingPolicy()
        };

        public AuthenticationController(IOptions<AuthenticationConfiguration> authenticationConfiguration,
            Narato.Correlations.Http.Interfaces.IHttpClientFactory httpClientFactory, ILogger<AuthenticationController> logger)
        {
            _authenticationConfiguration = authenticationConfiguration.Value;
            _httpClient = httpClientFactory.Create();
            _logger = logger;
        }

        [HttpPost("authentication/token")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTokenAsync([FromBody] AuthenticationTokenRequestDto requestDto)
        {
            var path = $"{_authenticationConfiguration.Instance}/{_authenticationConfiguration.TenantId}/oauth2/v2.0/token";
            var form = new Dictionary<string, string>();
            form.Add("grant_type", "authorization_code");
            form.Add("client_id", _authenticationConfiguration.ClientId);
            form.Add("client_secret", _authenticationConfiguration.ClientSecret);
            form.Add("code", requestDto.AuthorizationCode);
            form.Add("redirect_uri", requestDto.RedirectUri);            form.Add("scope", requestDto.Scope);

            var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = new FormUrlEncodedContent(form) };

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("Token response received: [{statusCode}] {message}", response.StatusCode, responseString);

            response.EnsureSuccessStatusCode();

            if (string.IsNullOrEmpty(responseString))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            else
            {
                return Ok(JsonSerializer.Deserialize<AuthenticationTokenResponseDto>(responseString, jsonSerializerOptions));
            }

        }

        [HttpPost("authentication/refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] AuthenticationTokenRefreshRequestDto requestDto)
        {
            var path = $"{_authenticationConfiguration.Instance}/{_authenticationConfiguration.TenantId}/oauth2/v2.0/token";
            var form = new Dictionary<string, string>();
            form.Add("grant_type", "refresh_token");
            form.Add("client_id", _authenticationConfiguration.ClientId);
            form.Add("client_secret", _authenticationConfiguration.ClientSecret);
            form.Add("refresh_token", requestDto.RefreshToken);
            form.Add("redirect_uri", requestDto.RedirectUri);
            form.Add("scope", requestDto.Scope);

            var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = new FormUrlEncodedContent(form) };

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("Refresh token response received: [{statusCode}] {message}", response.StatusCode, responseString);

            response.EnsureSuccessStatusCode();

            if (string.IsNullOrEmpty(responseString))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            else
            {
                return Ok(JsonSerializer.Deserialize<AuthenticationTokenResponseDto>(responseString, jsonSerializerOptions));
            }
        }

    }
}