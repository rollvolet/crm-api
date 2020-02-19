using System;
using System.Collections.Generic;
using Rollvolet.CRM.Domain.Models.Interfaces;

namespace Rollvolet.CRM.Domain.Models
{
    public class Intervention : ICaseRelated
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string Comment { get; set; }
        public DateTime? PlanningDate { get; set; }
        public string PlanningMsObjectId { get; set; }

        public Customer Customer { get; set; }
        public Building Building { get; set; }
        public Contact Contact { get; set; }
        public Invoice Invoice { get; set; }
        public Request FollowUpRequest { get; set; }
        public WayOfEntry WayOfEntry { get; set; }
        public IEnumerable<Employee> Technicians { get; set; }
    }
}