using System;
using System.Collections.Generic;
using System.Linq;
using Rollvolet.CRM.Domain.Models.Interfaces;

namespace Rollvolet.CRM.Domain.Models
{
    public class Intervention : ICaseRelated
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string Comment { get; set; }
        public DateTime? CancellationDate { get; set; }

        public Customer Customer { get; set; }
        public Building Building { get; set; }
        public Contact Contact { get; set; }
        public Order Origin { get; set; }
        public Invoice Invoice { get; set; }
        public Request FollowUpRequest { get; set; }
        public WayOfEntry WayOfEntry { get; set; }
        public Employee Employee { get; set; }
        public PlanningEvent PlanningEvent { get; set; }
        public IEnumerable<Employee> Technicians { get; set; }

        public string CalendarSubject
        {
            get
            {
                var entity = Building != null ? (CustomerEntity) Building : Customer;
                var addressLines = string.Join(",", new string[3] { entity.Address1, entity.Address2, entity.Address3 }.Where(a => !String.IsNullOrEmpty(a)));
                var address = $"{entity.PostalCode} {entity.City} ({addressLines})";
                return $"{Customer.Name} - {address} ** IR{Id} ({Comment})";
            }
        }
    }
}