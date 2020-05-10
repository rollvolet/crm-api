using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class DepositInvoiceHub
    {
        [Column("VoorschotId")]
        public int Id { get; set; }

        [Column("KlantID")]
        public int? CustomerId { get; set; }

        [Column("OfferteID")]
        public int? OrderId { get; set; }

        [Column("VoorschotFactuurID")]
        public int? DepositInvoiceId { get; set; }

        [Column("FactuurID")]
        public int? InvoiceId { get; set; }

        [Column("Datum")]
        public DateTime? Date { get; set; }


        // Include resources
        public Customer Customer { get; set; }
        public Order Order { get; set; }
        public Invoice DepositInvoice { get; set; }
        public Invoice Invoice { get; set; }
    }
}