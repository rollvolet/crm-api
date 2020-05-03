using System;

namespace Rollvolet.CRM.APIContracts.DTO.CalendarEvents
{
    public class CalendarEventAttributesDto
    {
        public DateTime? VisitDate { get; set; }
        public string Period { get; set; }
        public string FromHour { get; set; }
        public string UntilHour { get; set; }
        public string CalendarSubject { get; set; }
        public string CalendarId { get; set; }
        public string MsObjectId { get; set; }
    }
}