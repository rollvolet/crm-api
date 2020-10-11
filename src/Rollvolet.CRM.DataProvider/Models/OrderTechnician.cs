using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class OrderTechnician
    {
        [Column("OrderId")]
        public int OrderId { get; set; }

        [Column("EmployeeId")]
        public int EmployeeId { get; set; }

        public Order Order { get; set; }
        public Employee Employee { get; set; }
    }
}