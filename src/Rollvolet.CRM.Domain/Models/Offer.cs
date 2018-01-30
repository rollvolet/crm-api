using System;
using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public Request Request { get; set; }
        public Order Order { get; set; }
        public Customer Customer { get; set; }
        public Building Building { get; set; }
        public Contact Contact { get; set; }
        public VatRate VatRate { get; set; }
        public SubmissionType SubmissionType { get; set; }
        public Product Product { get; set; }
        

        public string Number { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime OfferDate { get; set; }
        public double Amount { get; set; }
        public DateTime SubmissionDate { get; set; }
        public double ForeseenHours { get; set; }
        public double ForeseenNbOfPersons { get; set; }
        public string Comment { get; set; } 
        public string Reference { get; set; }
        public bool Canceled { get; set; }
        public DateTime Updated { get; set; }
    }
}