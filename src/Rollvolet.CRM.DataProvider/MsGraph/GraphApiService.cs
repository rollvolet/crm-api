
using Microsoft.Graph;
using Rollvolet.CRM.Domain.Contracts.MsGraph;
using System.Threading.Tasks;

namespace Rollvolet.CRM.DataProvider.MsGraph
{
    public class GraphApiService : IGraphApiService
    {
        private readonly IAuthenticationProvider _msGraphAuthenticationProvider;

        public GraphApiService(IAuthenticationProvider authenticationProvider)
        {
            _msGraphAuthenticationProvider = authenticationProvider;
        }

        public async Task<object> GetUserProfileAsync()
        {
            var client = new GraphServiceClient(_msGraphAuthenticationProvider);
            return await client.Me.Request().GetAsync();
        }
    }
}