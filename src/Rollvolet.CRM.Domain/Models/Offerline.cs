using System;
using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class Offerline
    {
        public int Id { get; set; }
        public Offer Offer { get; set; }
        public VatRate VatRate { get; set; }

        public int SequenceNumber { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }
    }
}