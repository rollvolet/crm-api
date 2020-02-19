using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class WorkingHour
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("FactuurId")]
        public int InvoiceId { get; set; }

        [Column("Datum")]
        public DateTime Date { get; set; }

        [Column("Technieker")]
        public string EmployeeName { get; set; }

        // @Deprecated   TODO remove not-NULL constraint in DB and deprecate this property
        [Column("Uren")]
        public decimal Hours { get; set; }


        // Included resources
        public Invoice Invoice { get; set; }
        public Employee Employee { get; set; }
    }
}