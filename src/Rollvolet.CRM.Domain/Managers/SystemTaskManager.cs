using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;

namespace Rollvolet.CRM.Domain.Managers
{
    public class SystemTaskManager : ISystemTaskManager
    {
        private readonly ISystemTaskDataProvider _systemTaskDataProvider;
        private readonly ILogger _logger;

        public SystemTaskManager(ISystemTaskDataProvider systemTaskDataProvider, ILogger<SystemTaskManager> logger)
        {
            _systemTaskDataProvider = systemTaskDataProvider;
            _logger = logger;
        }

        public async Task RecalculateSearchNames()
        {
            await _systemTaskDataProvider.RecalcalulateSearchNames();
        }

    }
}