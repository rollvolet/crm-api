using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class WayOfEntry
    {
        [Column("AanmeldingID")]
        public int Id { get; set; }

        [Column("AanmeldingOmschrijving")]
        public string Name { get; set; }
    }
  
}