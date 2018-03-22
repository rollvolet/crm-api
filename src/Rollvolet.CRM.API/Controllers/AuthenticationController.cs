using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Narato.Correlations.Http.Interfaces;
using Newtonsoft.Json;
using Rollvolet.CRM.API.Builders;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.API.Configuration;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    public class AuthenticationController : Controller
    {
        private AuthenticationConfiguration _authenticationConfiguration;
        private readonly ILogger _logger;
        private HttpClient _httpClient;
        
        public AuthenticationController(IOptions<AuthenticationConfiguration> authenticationConfiguration, 
            IHttpClientFactory httpClientFactory, ILogger<AuthenticationController> logger)
        {
            _authenticationConfiguration = authenticationConfiguration.Value;
            _httpClient = httpClientFactory.Create();
            _logger = logger;
        }

        [HttpPost("authentication/token")]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken([FromBody] AuthenticationTokenRequestDto requestDto)
        {
            var path = $"{_authenticationConfiguration.BaseUri}/{_authenticationConfiguration.TenantId}/oauth2/v2.0/token";
            var form = new Dictionary<string, string>();
            form.Add("grant_type", "authorization_code");
            form.Add("client_id", _authenticationConfiguration.ClientId);
            form.Add("code", requestDto.AuthorizationCode);
            form.Add("redirect_uri", _authenticationConfiguration.RedirectUri);
            form.Add("client_secret", _authenticationConfiguration.ClientSecret);
            form.Add("scope", requestDto.Scope);

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
                return Ok(JsonConvert.DeserializeObject<AuthenticationTokenResponseDto>(responseString));
            } 
            
        }

        [HttpPost("authentication/refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] AuthenticationTokenRefreshRequestDto requestDto)
        {
            var path = $"{_authenticationConfiguration.BaseUri}/{_authenticationConfiguration.TenantId}/oauth2/v2.0/token";
            var form = new Dictionary<string, string>();
            form.Add("grant_type", "refresh_token");
            form.Add("client_id", _authenticationConfiguration.ClientId);
            form.Add("refresh_token", requestDto.RefreshToken);
            form.Add("redirect_uri", requestDto.RedirectUri);
            form.Add("client_secret", _authenticationConfiguration.ClientSecret);
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
                return Ok(JsonConvert.DeserializeObject<AuthenticationTokenResponseDto>(responseString));
            }
        }

    }
} 