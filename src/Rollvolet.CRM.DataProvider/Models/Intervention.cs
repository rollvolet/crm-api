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

        [Column("FollowUpRequestId")]
        public int? FollowUpRequestId { get; set; }

        [Column("Date")]
        public DateTime Date { get; set; }

        [Column("Comment")]
        public string Comment { get; set; }

        [Column("PlanningDate")]
        public DateTime PlanningDate { get; set; }

        [Column("PlanningMsObjectId")]
        public string PlanningMsObjectId { get; set; }


        // Include resources
        public Customer Customer { get; set; }
        public WayOfEntry WayOfEntry { get; set; }
        public Invoice Invoice { get; set; }
        public Request FollowUpRequest { get; set; }

        public IEnumerable<InterventionTechinican> InterventionTechinicans { get; set; }


        // Manually included resources
        [NotMapped]
        public Building Building { get; set; }
        [NotMapped]
        public Contact Contact { get; set; }
    }
}