using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Customer : CustomerRecord
    {
        [Column("ID")]
        public int CustomerId { get; set; }
    }
}