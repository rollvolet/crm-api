using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Rollvolet.CRM.Domain.Configuration;
using Rollvolet.CRM.Domain.Contracts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Contracts.MsGraph;
using Rollvolet.CRM.Domain.Exceptions;

namespace Rollvolet.CRM.DataProvider.MsGraph
{
    public class GraphApiFileStorageService : IFileStorageService, IGraphApiSystemTaskService
    {
        private readonly IGraphServiceClient _client;
        private readonly IOfferDataProvider _offerDataProvider;
        private readonly FileStorageConfiguration _fileStorageConfig;
        private readonly ILogger _logger;

        public GraphApiFileStorageService(IAuthenticationProvider authenticationProvider,
                                            IOfferDataProvider offerDataProvider,
                                            IOptions<FileStorageConfiguration> fileStorageConfiguration,
                                            ILogger<GraphApiFileStorageService> logger)
        {
            _client = new GraphServiceClient(authenticationProvider);
            _offerDataProvider = offerDataProvider;
            _fileStorageConfig = fileStorageConfiguration.Value;
            _logger = logger;
        }

        public async Task<string> CreateDirectoryPathAsync(string[] directories, string parent = null)
        {
            // TODO send requests in batch to the Graph API
            // See https://docs.microsoft.com/en-gb/graph/json-batching?context=graph%2Fapi%2F1.0&view=graph-rest-1.0
            foreach (var directory in directories)
            {
                if (directory.Trim().Length > 0)
                    parent = await CreateDirectoryAsync(directory, parent);
            }
            return parent;
        }

        public async Task<string> CreateDirectoryAsync(string directory, string parent = null)
        {
            var driveItem = new DriveItem
            {
                Name = directory,
                Folder = new Folder {},
                AdditionalData = new Dictionary<string, object>()
                {
                    { "@microsoft.graph.conflictBehavior", "replace" }
                }
            };
            var parentFolder = parent == null ? "root folder" : parent;
            _logger.LogDebug($"Creating directory {directory} on drive {_fileStorageConfig.DriveId} in {parentFolder}");

            try
            {
                var root = _client.Drives[_fileStorageConfig.DriveId].Root;
                var children = parent != null ? root.ItemWithPath(parent).Children : root.Children;
                await children.Request().AddAsync(driveItem);
                return $"{parent}/{directory}";
            }
            catch (ServiceException ex)
            {
                _logger.LogWarning($"Creating directory {directory} on drive {_fileStorageConfig.DriveId} in {parentFolder} failed: {ex.ToString()}");
                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(Microsoft.Identity.Client.MsalUiRequiredException))
                    throw ex.InnerException;
                else
                    throw ex;
            }
        }

        public string EnsureDirectory(string directory)
        {
            directory.TrimEnd('/');
            // TODO check whether folder exists via the Graph API
            return directory;
        }

        public async Task UploadDocumentAsync(string directory, string fileName, Stream content)
        {
            // Based on example at https://docs.microsoft.com/en-us/graph/sdks/large-file-upload?tabs=csharp
            var filePath = $"{directory}/{fileName}";
            var uploadProps = new DriveItemUploadableProperties
            {
                ODataType = null,
                AdditionalData = new Dictionary<string, object>
                {
                    { "@microsoft.graph.conflictBehavior", "replace" }
                }
            };

            // Create the upload session
            // itemPath does not need to be a path to an existing item
            var uploadSession = await _client.Drives[_fileStorageConfig.DriveId].Root
                .ItemWithPath(filePath)
                .CreateUploadSession(uploadProps)
                .Request()
                .PostAsync();

            // Max slice size must be a multiple of 320 KiB
            int maxSliceSize = 320 * 1024;
            var fileUploadTask = new LargeFileUploadTask<DriveItem>(uploadSession, content, maxSliceSize);

            _logger.LogInformation($"Starting upload file to drive {_fileStorageConfig.DriveId} on path {filePath}");

            // Create a callback that is invoked after each slice is uploaded
            IProgress<long> progress = new Progress<long>(progress => {
                _logger.LogDebug($"Uploaded {progress} bytes of stream");
            });

            try
            {
                var uploadResult = await fileUploadTask.UploadAsync(progress);

                if (uploadResult.UploadSucceeded)
                {
                    _logger.LogInformation($"Uploading file to drive {_fileStorageConfig.DriveId} on path {directory} succeeded (item id: {uploadResult.ItemResponse.Id}");
                }
                else
                {
                    _logger.LogError($"Uploading file to drive {_fileStorageConfig.DriveId} on path {directory} failed.");
                    throw new CodedException("UploadFailed", "Upload failed", $"Uploading file to drive {_fileStorageConfig.DriveId} on path {directory} failed.");
                }
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError($"Uploading file to drive {_fileStorageConfig.DriveId} on path {directory} failed: {ex.ToString()}");
                throw ex;
            }
            catch (ServiceException ex)
            {
                _logger.LogError($"Uploading file to drive {_fileStorageConfig.DriveId} on path {directory} failed: {ex.ToString()}");
                throw ex;
            }
        }

        public async Task<Stream> DownloadDocumentAsync(string filePath)
        {
            try
            {
                _logger.LogDebug($"Downloading file from drive {_fileStorageConfig.DriveId} on path {filePath}");
                return await _client.Drives[_fileStorageConfig.DriveId].Root
                    .ItemWithPath(filePath)
                    .Content
                    .Request()
                    .GetAsync();
            }
            catch (NullReferenceException)
            {
                _logger.LogInformation($"Cannot find file on drive {_fileStorageConfig.DriveId} on path {filePath}");
                throw new EntityNotFoundException();
            }
            catch (ServiceException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning($"Cannot find file on drive {_fileStorageConfig.DriveId} on path {filePath}: {ex.ToString()}");
                    throw new EntityNotFoundException();
                }
                else
                {
                    _logger.LogError($"Downloading file from drive {_fileStorageConfig.DriveId} on path {filePath} failed: {ex.ToString()}");
                    throw ex;
                }
            }
        }

        public async Task<string> FindDocumentAsync(string directory, string search)
        {
            var matchingFiles = await _client.Drives[_fileStorageConfig.DriveId].Root
                    .ItemWithPath(directory)
                    .Search(search)
                    .Request()
                    .Top(1)
                    .GetAsync();

            foreach (var driveItem in matchingFiles)
            {
                return $"{directory}/{driveItem.Name}";
            }

            return null;
        }

        public async Task CopyDocumentAsync(string sourcePath, string directory, string fileName, bool isRetry = false)
        {
            try
            {
                var parent = await _client.Drives[_fileStorageConfig.DriveId].Root
                    .ItemWithPath(directory)
                    .Request()
                    .GetAsync();

                var parentReference = new ItemReference
                {
                    DriveId = _fileStorageConfig.DriveId,
                    Id = parent.Id
                };

                await _client.Drives[_fileStorageConfig.DriveId].Root
                    .ItemWithPath(sourcePath)
                    .Copy(fileName, parentReference)
                    .Request()
                    .PostAsync();
            }
            catch (NullReferenceException)
            {
                _logger.LogInformation($"File on path {sourcePath} not found. nothing to copy.");
            }
            catch (ServiceException ex)
            {
                if (!isRetry && ex.StatusCode == HttpStatusCode.NotFound)
                {
                    var directories = directory.Split("/");
                    await CreateDirectoryPathAsync(directories);
                    await CopyDocumentAsync(sourcePath, directory, fileName, true); // folder has been created, try again...
                }
                else
                {
                    throw ex;
                }
            }
        }

        public async Task RemoveDocumentAsync(string filePath)
        {
            try
            {
                _logger.LogDebug($"Removing file from drive {_fileStorageConfig.DriveId} on path {filePath}");
                await _client.Drives[_fileStorageConfig.DriveId].Root
                    .ItemWithPath(filePath)
                    .Request()
                    .DeleteAsync();
            }
            catch (NullReferenceException)
            {
                _logger.LogInformation($"File on path {filePath} not found. nothing to delete.");
            }
            catch (ServiceException ex)
            {
                _logger.LogWarning($"Deleting file from drive {_fileStorageConfig.DriveId} on path {filePath} failed: {ex.ToString()}");
            }
        }

        public async Task RenameOfferDocumentsAsync(string directory, int pageSize = 999)
        {
            var fileNameRegex = new Regex(@"^([0-9]{2})([0-9]{2})([0-9]{2})_*([0-9]{2})_*(.*)\.pdf$", RegexOptions.Multiline);

            Dictionary<string, string> driveItems = new Dictionary<string, string>();

            try {
                var driveItemsPage = await _client.Drives[_fileStorageConfig.DriveId].Root
                    .ItemWithPath(directory)
                    .Children
                    .Request()
                    .Top(pageSize)
                    .GetAsync();

                var pageIterator = PageIterator<DriveItem>.CreatePageIterator(
                    _client,
                    driveItemsPage,
                    (driveItem) => 
                    {
                        driveItems.Add(driveItem.Id, driveItem.Name);
                        return true;
                    }
                );

                await pageIterator.IterateAsync();

                _logger.LogInformation($"Found {driveItems.Count} drive items in {directory} that need to be renamed.");

                foreach (var entry in driveItems)
                {
                    string driveItemId = entry.Key;
                    string fileName = entry.Value;

                    var match = fileNameRegex.Match(fileName);
                    if (match.Success)
                    {
                        var offerNumber = $"{match.Groups[1].Value}/{match.Groups[2].Value}/{match.Groups[3].Value}/{match.Groups[4].Value}";
                        try
                        {
                            var offer = await _offerDataProvider.GetByOfferNumberAsync(offerNumber);
                            var postfix = String.IsNullOrEmpty(match.Groups[5].Value) ? "" : $"_{match.Groups[5].Value}";
                            var adFileName = $"AD{offer.RequestNumber}{postfix}.pdf";

                            var renamedDriveItem = new DriveItem { Name = adFileName };
                            await _client.Drives[_fileStorageConfig.DriveId]
                                .Items[driveItemId]
                                .Request()
                                .UpdateAsync(renamedDriveItem);
                        }
                        catch (EntityNotFoundException)
                        {
                            _logger.LogWarning($"Cannot find offer with number {offerNumber}. Unable to rename document.");
                    
                        }
                        catch (ServiceException ex)
                        {
                            _logger.LogWarning($"Something went wrong renaming file {fileName}: {ex.ToString()}");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Ignoring file {fileName} from drive {_fileStorageConfig.DriveId} in directory {directory} because it doesn't have a standardized offer document name");
                    }
                }
            }
            catch (ServiceException)
            {
                _logger.LogWarning($"Something went wrong listing files in {directory}");
            }


        }
    }
}
