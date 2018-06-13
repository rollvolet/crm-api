using System;
using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class Visit
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public Request Request { get; set; }
        public bool OfferExpected { get; set; }
        public string Visitor { get; set; }
        public DateTime? VisitDate { get; set; }
        public string Period { get; set; }
        public string Comment { get; set; }
        public string CalendarSubject { get; set; }
        public string CalendarId { get; set; }
        public string MsObjectId { get; set; }

        public bool IsMasteredByAccess {
            get {
                return CalendarId != null;
            }
        }
    }
}