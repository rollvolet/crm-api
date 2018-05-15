using System;
using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class WorkingHour
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        // Included resources
        public Invoice Invoice { get; set; }
        public Employee Employee { get; set; }
    }
}