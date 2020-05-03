using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.Domain.Managers
{
    public class ErrorNotificationManager : IErrorNotificationManager
    {
        private readonly ILogger _logger;

        public ErrorNotificationManager(ILogger<ErrorNotificationManager> logger)
        {
            _logger = logger;
        }

        public async Task<ErrorNotification> CreateAsync(ErrorNotification errorNotification)
        {
            await Task.Run( () => {
                var json = JsonSerializer.Serialize(errorNotification);
               _logger.LogError("{Error}", json);
            });
            return errorNotification;
        }
    }
}