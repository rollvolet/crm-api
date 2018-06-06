using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Visit
    {
        [Column("BezoekId")]
        public int Id { get; set; }

        [Column("AanvraagId")]
        public int? RequestId { get; set; }

        [Column("Bezoeker")]
        public string Visitor { get; set; }

        [Column("Bezoekdatum")]
        public DateTime? VisitDate { get; set; }

        [Column("OfferteVerwacht")]
        public bool OfferExpected { get; set; }

        [Column("KlantID")]
        public int? CustomerId { get; set; }

        [Column("GebouwID")]
        public int? RelativeBuildingId { get; set; }

        [Column("ContactID")]
        public int? RelativeContactId { get; set; }

        [Column("Opmerking")]
        public string Comment { get; set; }

        [Column("AfspraakOnderwerp")]
        public string CalendarSubject { get; set; }

        [Column("AfspraakID")]
        public string CalendarId { get; set; }

        [Column("MsObjectId")]
        public string MsObjectId { get; set; }


        // Included resources
        public Request Request { get; set; }
        public Customer Customer { get; set; }


        // Embedded properties
        [Column("Gemeente")]
        public string EmbeddedCity { get; set; }
    }
}