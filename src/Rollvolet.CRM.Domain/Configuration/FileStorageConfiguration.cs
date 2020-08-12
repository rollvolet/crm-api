namespace Rollvolet.CRM.Domain.Configuration
{
    public class FileStorageConfiguration
    {
        public string Locaction { get; set; } // 'local' or 'cloud'
        public string DriveId { get; set; }  // only required for 'cloud'
    }
}