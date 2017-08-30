using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Contact : CustomerRecord
    {
        [Column("ParentID")]
        public int CustomerId { get; set; }

        [Column("ID")]
        public int ContactId { get; set; }
    }
}