using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Payment
    {
        [Column("Type")]
        public string Type { get; set; }
        
        [Column("Kode")]
        public string Id { get; set; }
        
        [Column("Waarde")]
        public string Name { get; set; }
    }
  
}