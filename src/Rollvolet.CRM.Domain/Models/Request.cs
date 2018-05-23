using System;
using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class Request
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public Building Building { get; set; }
        public Contact Contact { get; set; }
        public Visit Visit { get; set; }
        public Offer Offer { get; set; }

        public bool RequiresVisit { get; set; }
        public WayOfEntry WayOfEntry { get; set; }
        public DateTime? RequestDate { get; set; }
        public string Comment { get; set; }
        public string Employee { get; set; }
    }
}