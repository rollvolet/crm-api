using System;

namespace Rollvolet.CRM.Domain.Models
{
    public class CalendarEvent
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public Request Request { get; set; }
        public DateTime? VisitDate { get; set; }
        public string Period { get; set; }
        public string FromHour { get; set; }
        public string UntilHour { get; set; }
        public string CalendarSubject { get; set; }
        public string CalendarId { get; set; }
        public string MsObjectId { get; set; }

        public bool IsMasteredByAccess {
            get
            {
                return CalendarId != null;
            }
        }

        public bool RequiresFromHour {
            get
            {
                return Period == "vanaf" || Period == "bepaald uur" || Period == "stipt uur" || Period == "benaderend uur" || Period == "van-tot";
            }
        }

        public bool RequiresUntilHour {
            get
            {
                return Period == "van-tot";
            }
        }
    }
}