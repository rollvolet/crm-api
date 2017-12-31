using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Tag
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Keyword")]
        public string Name { get; set; }


        // Included resources
        public IEnumerable<CustomerTag> CustomerTags { get; set; }
    }
  
}