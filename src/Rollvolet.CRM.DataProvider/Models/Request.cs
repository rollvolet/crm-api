using System;
using System.ComponentModel.DataAnnotations.Schema;
using Rollvolet.CRM.DataProvider.Models.Interfaces;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Request : ICaseRelated
    {
        [Column("AanvraagID")]
        public int Id { get; set; }

        [Column("KlantID")]
        public int? CustomerId { get; set; }

        [Column("GebouwID")]
        public int? RelativeBuildingId { get; set; }

        [Column("ContactID")]
        public int? RelativeContactId { get; set; }

        [Column("Bezoek")]
        public bool RequiresVisit { get; set; }

        [Column("Bezoeker")]
        public string Visitor { get; set; }

        [Column("Bediende")]
        public string Employee { get; set; }

        [Column("AanmeldingID")]
        public int? WayOfEntryId { get; set; }

        [Column("OriginId")]
        public int? OriginId { get; set; }

        [Column("Aanvraagdatum")]
        public DateTime RequestDate { get; set; }

        [Column("Beschrijving")]
        public string Description { get; set; }

        [Column("Opmerking")]
        public string Comment { get; set; }

        // Kept in sync with calendar-event date by frontend.
        // Still required in SQL DB for reporting purposes.
        // Can be removed once requests are converted to triplestore.
        [Column("VisitDate")]
        public DateTime? VisitDate { get; set; }

        [Column("CancellationDate")]
        public DateTime? CancellationDate { get; set; }

        [Column("CancellationReason")]
        public string CancellationReason { get; set; }


        // Include resources
        public Customer Customer { get; set; }
        public Building Building { get; set; }
        public Contact Contact { get; set; }
        public WayOfEntry WayOfEntry { get; set; }
        public Offer Offer { get; set; }
        public Intervention Origin { get; set; }


        // Embedded properties
        [Column("Gemeente")]
        public string EmbeddedCity { get; set; }
    }
}