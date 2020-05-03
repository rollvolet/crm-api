using System;

namespace Rollvolet.CRM.APIContracts.DTO.AccountancyExports
{
    public class AccountancyExportAttributesDto
    {
        public DateTime Date { get; set; }
        public int? FromNumber { get; set; }
        public int? UntilNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? UntilDate { get; set; }
        public bool IsDryRun { get; set; }
    }
}