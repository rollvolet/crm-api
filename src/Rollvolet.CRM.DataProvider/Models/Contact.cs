using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Contact : CustomerRecord
    {
        [Column("ParentID")]
        public int CustomerId { get; set; }

        // Include resources
        public Customer Customer { get; set; }
    }
}