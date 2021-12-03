using System;

namespace Rollvolet.CRM.APIContracts.DTO.Reports
{
    public class OutstandingJobReportAttributesDto
    {
        public double TotalHours { get; set; }
        public int NumberOverdue { get; set; }
    }
}