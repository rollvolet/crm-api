using System;

namespace Rollvolet.CRM.Domain.Models
{
    public class PlanningEvent
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string MsObjectId { get; set; }
        public string Subject { get; set; }
        public string Period { get; set; }
        public string FromHour { get; set; }
        public string UntilHour { get; set; }
        public bool IsNotAvailableInCalendar { get; set; }

        public Intervention Intervention { get; set; }
        public Order Order { get; set; }

        public PlanningEvent()
        {
            IsNotAvailableInCalendar = false;
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

        public bool IsPlanned {
            get
            {
                return MsObjectId != null;
            }
        }
    }
}