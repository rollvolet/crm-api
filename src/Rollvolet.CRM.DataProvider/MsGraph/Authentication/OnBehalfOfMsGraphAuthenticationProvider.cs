using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Graph;
using Rollvolet.CRM.Domain.Authentication.Interfaces;
using Rollvolet.CRM.Domain.Configuration;
using VDS.RDF.Query;

namespace Rollvolet.CRM.DataProvider.MsGraph.Authentication
{
    public class OnBehalfOfMsGraphAuthenticationProvider : IAuthenticationProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfidentialClientApplicationProvider _applicationProvider;
        private readonly SparqlRemoteEndpoint _sparqlEndpoint;
        private readonly ILogger _logger;

        public OnBehalfOfMsGraphAuthenticationProvider(IHttpContextAccessor httpContextAccessor,
            IConfidentialClientApplicationProvider applicationProvider,
            IOptions<SparqlConfiguration> sparqlConfiguration,
            ILogger<OnBehalfOfMsGraphAuthenticationProvider> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _applicationProvider = applicationProvider;
            _sparqlEndpoint = new SparqlRemoteEndpoint(new Uri(sparqlConfiguration.Value.Endpoint));
            _logger = logger;
        }

        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var sessionHeader = _httpContextAccessor.HttpContext.Request.Headers["mu-session-id"];

            if (!StringValues.IsNullOrEmpty(sessionHeader))
            {
                var session = sessionHeader.ToString();

                _logger.LogDebug($"Retrieved session <{session}> from request headers");
                var query = $@"
                    PREFIX oauth: <http://data.rollvolet.be/vocabularies/oauth-2.0/>
                    SELECT ?accessToken
                    WHERE {{
                        GRAPH <http://mu.semte.ch/graphs/sessions> {{
                            ?oauthSession oauth:authenticates <{session}> ;
                                    oauth:tokenValue ?accessToken .
                        }}
                    }} LIMIT 1";
                var resultSet = _sparqlEndpoint.QueryWithResultSet(query);

                if (resultSet.Count > 0)
                {
                    var accessToken = resultSet.Results.First()["accessToken"].ToString();
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
                else
                {
                    _logger.LogWarning($"No access token found for session <{session}>");
                    throw new AuthenticationException();
                }
            }
            else
            {
                _logger.LogWarning($"No session header found on request");
                throw new AuthenticationException();
            }
        }
    }
}