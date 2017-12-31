using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class CustomerTag
    {
        [Column("DataID")]
        public int CustomerId { get; set; }

        [Column("KeywordID")]
        public int TagId { get; set; }

        public Customer Customer { get; set; }
        public Tag Tag { get; set; }
    }
}