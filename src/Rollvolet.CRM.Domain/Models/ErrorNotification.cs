namespace Rollvolet.CRM.Domain.Models
{
    public class ErrorNotification
    {
        public string Version { get; set; }
        public string File { get; set; }
        public int? LineNumber { get; set; }
        public int? ColumnNumber { get; set; }
        public string CurrentUrl { get; set; }
        public string CurrentPath { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
    }
}