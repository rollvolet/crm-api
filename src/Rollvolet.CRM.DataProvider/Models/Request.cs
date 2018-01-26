using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Request
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
        
        [Column("AanmeldingID")]
        public int? WayOfEntryId { get; set; }

        [Column("Aanvraagdatum")]
        public DateTime RequestDate { get; set; }

        [Column("Opmerking")]
        public string Comment { get; set; }    

        [Column("Bediende")]
        public string Employee { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("UpdTimestamp")]
        public DateTime Updated { get; set; }         


        // Include resources
        public Customer Customer { get; set; }
        public WayOfEntry WayOfEntry { get; set; }
        public Visit Visit { get; set; }

        // Manually included resources
        [NotMapped]
        public Building Building { get; set; }
        [NotMapped]
        public Contact Contact { get; set; }


        // Embedded properties
        [Column("Gemeente")]
        public string EmbeddedCity { get; set; }


        public class BuildingTuple
        {
            public Request Request { get; set; }
            public Building Building { get; set; }
        }

        public class Triplet
        {
            public Request Request { get; set; }
            public Building Building { get; set; }
            public Contact Contact { get; set; }
        }
    }
}