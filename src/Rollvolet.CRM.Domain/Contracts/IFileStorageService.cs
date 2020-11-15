using System.IO;
using System.Threading.Tasks;

namespace Rollvolet.CRM.Domain.Contracts
{
    public interface IFileStorageService
    {
        Task<string> CreateDirectoryPathAsync(string[] directories, string parent = null);
        Task<string> CreateDirectoryAsync(string directory, string parent = null);
        string EnsureDirectory(string directory);
        Task UploadDocumentAsync(string directory, string fileName, Stream content);
        Task<Stream> DownloadDocumentAsync(string filePath);
        Task<string> FindDocumentAsync(string directory, string search);
        Task CopyDocumentAsync(string sourcePath, string directory, string fileName, bool isRetry = false);
        Task RemoveDocumentAsync(string filePath);
    }
}