using System;
using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }
        public Offer Offer { get; set; }
        public Invoice Invoice { get; set; }
        public Customer Customer { get; set; }
        public Building Building { get; set; }
        public Contact Contact { get; set; }
        public VatRate VatRate { get; set; }
        public IEnumerable<Deposit> Deposits { get; set; }
        public IEnumerable<DepositInvoice> DepositInvoices { get; set; }
        
        // TODO add calendar event
        
        public DateTime? OrderDate { get; set; }
        public string OfferNumber { get; set; }        
        public double? Amount { get; set; }
        public bool DepositRequired { get; set; }
        public bool HasProductionTicket { get; set; }
        public bool MustBeInstalled { get; set; }
        public bool IsReady { get; set; }
        public bool MustBeDelivered { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public float? ScheduledHours { get; set; }
        public float? ScheduledNbOfPersons { get; set; }  
        public float? InvoicableHours { get; set; }
        public float? InvoicableNbOfPersons { get; set; }  
        public string Comment { get; set; }                 
        public bool Canceled { get; set; }
        public string CancellationReason { get; set; } 
    }
}