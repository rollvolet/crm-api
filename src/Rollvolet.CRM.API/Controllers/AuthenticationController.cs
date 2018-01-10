using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Rollvolet.CRM.API.Builders;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{    
    public class AuthenticationController : Controller
    {
        public AuthenticationController()
        {
        }

        [HttpPost("authentication/token")]
        public async Task<IActionResult> GetToken([FromBody] AuthenticationRequestDto authentication)
        {
            var authority = "https://login.microsoftonline.com/3e9b8827-39f2-4fb4-9bc1-f8a200aaea79";
            var authenticationContext = new AuthenticationContext(authority);

            var clientId = "de1c3029-8d4c-46ab-b3a7-717cac926280";
            var clientSecret = "onjaNAUBpi04tmHuIJLT97xzao2VVzHh8KOCwJ0jUJU=";
            var credential = new ClientCredential(clientId, clientSecret);

            var authenticationResult = await authenticationContext.AcquireTokenByAuthorizationCodeAsync(
                                        authentication.AuthorizationCode, new Uri(authentication.RedirectUri), credential, clientId);

            return Ok(authenticationResult);
        }

    }
} 