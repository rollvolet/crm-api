using Microsoft.Identity.Client;

namespace Rollvolet.CRM.Domain.Authentication.Interfaces
{
    public interface IConfidentialClientApplicationProvider
    {
        IConfidentialClientApplication GetApplication();
    }
}
