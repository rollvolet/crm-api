using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Invoiceline
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("SequenceNumber")]
        public int SequenceNumber { get; set; }

        [Column("Amount")]
        public double? Amount { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("VatRateId")]
        public int? VatRateId { get; set; }

        [Column("OrderId")]
        public int? OrderId { get; set; }

        [Column("InvoiceId")]
        public int? InvoiceId { get; set; }

        [Column("Currency")]
        public string Currency { get; set; }

        // Include resources
        public Order Order  { get; set; }
        public Invoice Invoice { get; set; }
        public VatRate VatRate { get; set; }
    }
}