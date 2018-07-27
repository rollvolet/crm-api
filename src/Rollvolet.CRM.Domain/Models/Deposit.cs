using System;
using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class Deposit
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public Order Order { get; set; }
        public Invoice Invoice { get; set; }
        public int SequenceNumber { get; set; }
        public double? Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public Payment Payment { get; set; }

    }
}