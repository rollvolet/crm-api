using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rollvolet.CRM.Domain.Configuration;
using Rollvolet.CRM.Domain.Contracts;
using Rollvolet.CRM.Domain.Exceptions;

namespace Rollvolet.CRM.DataProvider.MsGraph
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly FileStorageConfiguration _fileStorageConfig;
        private readonly ILogger _logger;

        public LocalFileStorageService(IOptions<FileStorageConfiguration> fileStorageConfiguration,
                                            ILogger<LocalFileStorageService> logger)
        {
            _fileStorageConfig = fileStorageConfiguration.Value;
            _logger = logger;
        }

        public async Task<string> CreateDirectoryPathAsync(string[] directories, string parent = null)
        {
            return await Task.Run(() => {
                var path = String.Join(Path.DirectorySeparatorChar, directories);
                if (parent != null)
                  path = $"{parent}{Path.DirectorySeparatorChar}{path}";
                Directory.CreateDirectory(path);
                return path;
            });
        }

        public async Task<string> CreateDirectoryAsync(string directory, string parent = null)
        {
            return await Task.Run(() => {
                var path = directory;
                if (parent != null)
                  path = $"{parent}{Path.DirectorySeparatorChar}{path}";
                Directory.CreateDirectory(path);
                return path;
            });
        }

        public string EnsureDirectory(string directory)
        {
            if (!directory.EndsWith(Path.DirectorySeparatorChar))
                directory += Path.DirectorySeparatorChar;
            Directory.CreateDirectory(directory);
            return directory;
        }

        public async Task UploadDocumentAsync(string directory, string fileName, Stream content)
        {
            var filePath = $"{directory}{Path.DirectorySeparatorChar}{fileName}";
            await Task.Run(() => {
                using (var fileStream = File.Create(filePath))
                {
                    content.Seek(0, SeekOrigin.Begin);
                    content.CopyTo(fileStream);
                }
            });
        }

        public async Task<Stream> DownloadDocumentAsync(string filePath)
        {
            return await Task.Run(() => {
                try
                {
                    return new FileStream(filePath, FileMode.Open);
                }
                catch (FileNotFoundException)
                {
                    _logger.LogWarning("Cannot find document at {1}", filePath);
                    throw new EntityNotFoundException();
                }
            });
        }

        public async Task<string> FindDocumentAsync(string directory, string search)
        {
            return await Task.Run(() => {
                var searchWithWildcard = search + "*";
                var matchingFiles = Directory.GetFiles(directory, search);

                if (matchingFiles.Length > 0)
                    return matchingFiles[0];
                else
                    return null;
            });
        }

        public async Task CopyDocumentAsync(string sourcePath, string directory, string fileName, bool isRetry = false)
        {
            var filePath = $"{directory}{Path.DirectorySeparatorChar}{fileName}";
            await Task.Run(() => {
                File.Copy(sourcePath, filePath, true);
            });
        }

        public async Task RemoveDocumentAsync(string filePath)
        {
            await Task.Run(() => {
                try
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    else
                        _logger.LogDebug("File {0} not found and will not be removed.", filePath);
                }
                catch (Exception)
                {
                    _logger.LogDebug("Failed to remove file {0}.", filePath);
                }
            });
        }
    }
}
