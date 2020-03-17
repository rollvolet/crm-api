using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class PlanningEvent
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Date")]
        public DateTime? Date { get; set; }

        [Column("MsObjectId")]
        public string MsObjectId { get; set; }

        [Column("Subject")]
        public string Subject { get; set; }

        [Column("InterventionId")]
        public int InterventionId { get; set; }

        [Column("OrderId")]
        public int OrderId { get; set; }


        public Intervention Intervention { get; set; }
        public Order Order { get; set; }
    }
}