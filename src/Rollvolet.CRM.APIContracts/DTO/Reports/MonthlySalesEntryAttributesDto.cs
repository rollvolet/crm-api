using System;

namespace Rollvolet.CRM.APIContracts.DTO.Reports
{
    public class MonthlySalesEntryAttributesDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public double Amount { get; set; }
    }
}