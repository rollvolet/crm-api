using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class InvoiceSupplement
    {
        [Column("FactuurExtraID")]
        public int Id { get; set; }

        [Column("FactuurID")]
        public int? InvoiceId { get; set; }

        [Column("VolgNummer")]
        public short SequenceNumber { get; set; }

        [Column("Aantal")]
        public double? NbOfPieces { get; set; }

        [Column("EenheidId")]
        public int? UnitId { get; set; }

        [Column("NettoBedrag")]
        public double? Amount { get; set; }

        [Column("Omschrijving")]
        public string Description { get; set; }

        [Column("MuntEenheid")]
        public string Currency { get; set; }


        // Include resources
        public Invoice Invoice { get; set; }
        public ProductUnit Unit { get; set; }
    }
}