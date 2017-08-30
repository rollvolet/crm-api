using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Country
    {
        [Column("LandId")]
        public int Id { get; set; }
        [Column("Landcode")]
        public string Code { get; set; }
        [Column("LandOmschrijving")]
        public string Name { get; set; }
    }
  
}