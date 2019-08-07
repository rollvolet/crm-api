using System;

namespace Rollvolet.CRM.Domain.Models
{
    public class AccountancyExport
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public int? FromNumber { get; set; }
        public int? UntilNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? UntilDate { get; set; }
        public bool IsDryRun { get; set; }
    }
}