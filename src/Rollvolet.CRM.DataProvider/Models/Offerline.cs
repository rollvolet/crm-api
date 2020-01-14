using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Offerline
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

        [Column("OfferId")]
        public int OfferId { get; set; }

        [Column("Currency")]
        public string Currency { get; set; }

        // Include resources
        public Offer Offer  { get; set; }
        public VatRate VatRate { get; set; }
    }
}