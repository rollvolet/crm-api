using System;
using System.ComponentModel.DataAnnotations.Schema;
using Rollvolet.CRM.DataProvider.Models.Interfaces;

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

        [Column("Eenheid")]
        public string Unit { get; set; }

        [Column("NettoBedrag")]
        public double? Amount { get; set; }

        [Column("Omschrijving")]
        public string Description { get; set; }

        [Column("MuntEenheid")]
        public string Currency { get; set; }


        // Include resources
        public Invoice Invoice { get; set; }
    }
}