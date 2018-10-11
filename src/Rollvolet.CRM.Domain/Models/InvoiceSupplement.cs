using System;
using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class InvoiceSupplement
    {
        public int Id { get; set; }
        public int SequenceNumber { get; set; }
        public double? NbOfPieces { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }

        public Invoice Invoice { get; set; }

        public double? Total {
            get {
                var number = this.NbOfPieces ?? 1.0;
                var amount = this.Amount ?? 0.0;
                return number * amount;
            }
        }
    }
}