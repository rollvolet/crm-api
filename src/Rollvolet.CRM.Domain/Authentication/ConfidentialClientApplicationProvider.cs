using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Rollvolet.CRM.Domain.Authentication.Interfaces;

namespace Rollvolet.CRM.Domain.Authentication
{
    public class ConfidentialClientApplicationProvider : IConfidentialClientApplicationProvider
    {
        private readonly ConfidentialClientApplicationOptions _applicationOptions;
        private IConfidentialClientApplication _application;

        public ConfidentialClientApplicationProvider(IOptions<ConfidentialClientApplicationOptions> options)
        {
            _applicationOptions = options.Value;
        }

        public IConfidentialClientApplication GetApplication()
        {
            if (_application == null)
            {
                _application = ConfidentialClientApplicationBuilder
                      .CreateWithApplicationOptions(_applicationOptions)
                      .Build();
            }
            return _application;
        }
    }
}
