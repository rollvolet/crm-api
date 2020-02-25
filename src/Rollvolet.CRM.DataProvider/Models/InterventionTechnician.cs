using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class InterventionTechnician
    {
        [Column("InterventionId")]
        public int InterventionId { get; set; }

        [Column("EmployeeId")]
        public int EmployeeId { get; set; }

        public Intervention Intervention { get; set; }
        public Employee Employee { get; set; }
    }
}