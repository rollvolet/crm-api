using System;
using Rollvolet.CRM.Domain.Models.Interfaces;

namespace Rollvolet.CRM.Domain.Models
{
    public class Request : ICaseRelated
    {
        public int Id { get; set; }
        public bool RequiresVisit { get; set; }
        public DateTime? RequestDate { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public string Employee { get; set; }
        public string Visitor { get; set; }
        public DateTime? VisitDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string CancellationReason { get; set; }

        public Customer Customer { get; set; }
        public Building Building { get; set; }
        public Contact Contact { get; set; }
        public WayOfEntry WayOfEntry { get; set; }
        public Offer Offer { get; set; }
        public Intervention Origin { get; set; }
    }
}