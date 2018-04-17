using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class PostalCode
    {
        [Column("PostcodeId")]
        public int Id { get; set; }

        [Column("Postcode")]
        public string Code { get; set; }
        
        [Column("Gemeente")]
        public string Name { get; set; }
    }
  
}