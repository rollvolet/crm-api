using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class AccountancyExport
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("Datum")]
        public DateTime Date { get; set; }

        [Column("Vanaf")]
        public DateTime? FromDate { get; set; }

        [Column("Tot")]
        public DateTime? UntilDate { get; set; }

        [Column("FromNumber")]
        public int? FromNumber { get; set; }

        [Column("UntilNumber")]
        public int? UntilNumber { get; set; }

    }

}