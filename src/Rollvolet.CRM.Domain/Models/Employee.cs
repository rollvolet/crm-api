using System;
using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public short Type { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Initials { get; set; }
        public string Comment { get; set; }
        public double Active { get; set; }
        public string Function { get; set; }


        // Included resources
        public IEnumerable<WorkingHour> WorkingHours { get; set; }
    }
}