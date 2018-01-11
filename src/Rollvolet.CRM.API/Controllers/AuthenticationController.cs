using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Rollvolet.CRM.API.Builders;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.API.Configuration;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{    
    public class AuthenticationController : Controller
    {
        public AuthenticationConfiguration _authenticationConfiguration { get; set; }
        
        public AuthenticationController(IOptions<AuthenticationConfiguration> authenticationConfiguration)
        {
            _authenticationConfiguration = authenticationConfiguration.Value;
        }

        [HttpPost("authentication/token")]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken([FromBody] AuthenticationRequestDto authentication)
        {
            var authority = _authenticationConfiguration.Authority;
            var authenticationContext = new AuthenticationContext(authority);

            var clientId = _authenticationConfiguration.ClientId;
            var clientSecret = _authenticationConfiguration.ClientSecret;
            var credential = new ClientCredential(clientId, clientSecret);

            var authenticationResult = await authenticationContext.AcquireTokenByAuthorizationCodeAsync(
                                        authentication.AuthorizationCode, new Uri(authentication.RedirectUri), credential, clientId);

            return Ok(authenticationResult);
        }

    }
} 