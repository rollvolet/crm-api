using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Product
    {
        [Column("ProduktId")]
        public int Id { get; set; }

        [Column("ProduktOmschrijving")]
        public string Name { get; set; }
    }
  
}