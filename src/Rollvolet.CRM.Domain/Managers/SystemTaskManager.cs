using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rollvolet.CRM.Domain.Configuration;
using Rollvolet.CRM.Domain.Contracts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;

namespace Rollvolet.CRM.Domain.Managers
{
    public class SystemTaskManager : ISystemTaskManager
    {
        private readonly ISystemTaskDataProvider _systemTaskDataProvider;
        private readonly IFileStorageService _fileStorageService;
        private readonly string _offerStorageLocation;
        private readonly string _orderStorageLocation;
        private readonly string _deliveryNoteStorageLocation;
        private readonly string _generatedProductionTicketStorageLocation;
        private readonly string _receivedProductionTicketStorageLocation;
        private readonly ILogger _logger;

        public SystemTaskManager(ISystemTaskDataProvider systemTaskDataProvider,
                                    IFileStorageService fileStorageService,
                                    IOptions<DocumentGenerationConfiguration> documentGenerationConfiguration,
                                    ILogger<SystemTaskManager> logger)
        {
            _systemTaskDataProvider = systemTaskDataProvider;
            _fileStorageService = fileStorageService;
            var docConfig = documentGenerationConfiguration.Value;
            _offerStorageLocation = _fileStorageService.EnsureDirectory(docConfig.OfferStorageLocation);
            _orderStorageLocation = _fileStorageService.EnsureDirectory(docConfig.OrderStorageLocation);
            _deliveryNoteStorageLocation = _fileStorageService.EnsureDirectory(docConfig.DeliveryNoteStorageLocation);
            _generatedProductionTicketStorageLocation = _fileStorageService.EnsureDirectory(docConfig.GeneratedProductionTicketStorageLocation);
            _receivedProductionTicketStorageLocation = _fileStorageService.EnsureDirectory(docConfig.ReceivedProductionTicketStorageLocation);
            
            _logger = logger;
        }

        public async Task RenameOfferDocuments()
        {
            var locations = new string[] { 
                _offerStorageLocation, 
                _orderStorageLocation,
                _deliveryNoteStorageLocation, 
                _generatedProductionTicketStorageLocation,
                _receivedProductionTicketStorageLocation
            };
            await _systemTaskDataProvider.RenameOfferDocuments(locations);
        }

        public async Task RecalculateSearchNames()
        {
            await _systemTaskDataProvider.RecalcalulateSearchNames();
        }
    }
}