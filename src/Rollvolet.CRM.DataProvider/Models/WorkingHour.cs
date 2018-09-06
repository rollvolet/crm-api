using System;
using System.Collections.Generic;
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

        // DEPRECATED
        // [Column("Uren")]


        // Included resources
        public Invoice Invoice { get; set; }
        public Employee Employee { get; set; }
    }
}