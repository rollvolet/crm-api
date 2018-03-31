using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Employee
    {
        [Column("PersoneelId")]
        public int Id { get; set; }

        [Column("Type")]
        public short Type { get; set; }

        [Column("PNaam")]
        public string LastName { get; set; }

        [Column("Voornaam")]
        public string FirstName { get; set; }

        [Column("Initialen")]
        public string Initials { get; set; }

        [Column("Opmerking")]
        public string Comment { get; set; }

        [Column("InDienst")]
        public bool Active { get; set; }

        [Column("Aanvragen")]
        public string Function { get; set; }


        // Included resources
        public IEnumerable<WorkingHour> WorkingHours { get; set; }
    }
}