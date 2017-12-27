using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Building : CustomerRecord
    {
        [Column("ParentID")]
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }
    }
}