using System;
using System.Collections.Generic;
using Rollvolet.CRM.Domain.Models.Interfaces;

namespace Rollvolet.CRM.Domain.Models
{
    public class Offer : ICaseRelated
    {
        public int Id { get; set; }
        public Request Request { get; set; }
        public Order Order { get; set; }
        public Customer Customer { get; set; }
        public Building Building { get; set; }
        public Contact Contact { get; set; }
        public VatRate VatRate { get; set; }
        public IEnumerable<Offerline> Offerlines { get; set; }

        public string Number { get; set; }
        public int? RequestNumber { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime OfferDate { get; set; }
        public double? Amount { get; set; }
        public string Reference { get; set; }
        public string DocumentIntro { get; set; }
        public string DocumentOutro { get; set; }
        public string DocumentVersion { get; set; }
    }
}