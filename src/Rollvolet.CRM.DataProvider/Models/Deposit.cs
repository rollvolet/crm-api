using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Deposit
    {
        [Column("VoorschotId")]
        public int Id { get; set; }
        
        [Column("KlantID")]
        public int? CustomerId { get; set; }
        
        [Column("OfferteID")]
        public int? OrderId { get; set; }

        [Column("VolgNummer")]
        public short SequenceNumber { get; set; }        
        
        [Column("Betaalcode")]
        public string PaymentId { get; set; }

        [Column("Bedrag")]
        public double? Amount { get; set; }

        [Column("Datum")]
        public DateTime PaymentDate { get; set; }

        [Column("MuntEenheid")]
        public string Currency { get; set; }  

        [Column("IsVoorschot")]
        // when an invoice has been created already, the payment cannot be considered a deposit anymore
        public bool IsDeposit { get; set; }

        public Customer Customer { get; set; }
        public Order Order { get; set; }
        public Payment Payment { get; set; }
    }
  
}