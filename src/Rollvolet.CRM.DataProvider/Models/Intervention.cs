using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Rollvolet.CRM.DataProvider.Models.Interfaces;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Intervention : ICaseRelated
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("CustomerId")]
        public int? CustomerId { get; set; }

        [Column("BuildingId")]
        public int? RelativeBuildingId { get; set; }

        [Column("ContactId")]
        public int? RelativeContactId { get; set; }

        [Column("WayOfEntryId")]
        public int? WayOfEntryId { get; set; }

        [Column("OriginId")]
        public int? OriginId { get; set; }

        [Column("EmployeeId")]
        public int? EmployeeId { get; set; }

        [Column("Date")]
        public DateTime Date { get; set; }

        [Column("PlanningDate")]
        public DateTime? PlanningDate { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("Comment")]
        public string Comment { get; set; }

        [Column("NbOfPersons")]
        public float? NbOfPersons { get; set; }

        [Column("CancellationDate")]
        public DateTime? CancellationDate { get; set; }

        [Column("CancellationReason")]
        public string CancellationReason { get; set; }


        // Include resources
        public Customer Customer { get; set; }
        public Building Building { get; set; }
        public Contact Contact { get; set; }
        public WayOfEntry WayOfEntry { get; set; }
        public Order Origin { get; set; }
        public Invoice Invoice { get; set; }
        public Request FollowUpRequest { get; set; }
        public Employee Employee { get; set; }

        public ICollection<InterventionTechnician> InterventionTechnicians { get; set; }
    }
}