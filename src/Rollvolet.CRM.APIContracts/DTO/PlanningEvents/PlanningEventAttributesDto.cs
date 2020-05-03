using System;

namespace Rollvolet.CRM.APIContracts.DTO.PlanningEvents
{
    public class PlanningEventAttributesDto
    {
        public DateTime? Date { get; set; }
        public string MsObjectId { get; set; }
        public string Subject { get; set; }
        public string Period { get; set; }
        public string FromHour { get; set; }
        public string UntilHour { get; set; }
        public bool IsNotAvailableInCalendar { get; set; }
    }
}