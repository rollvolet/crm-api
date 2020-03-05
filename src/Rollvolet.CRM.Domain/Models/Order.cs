using System;
using System.Collections.Generic;
using Rollvolet.CRM.Domain.Models.Interfaces;

namespace Rollvolet.CRM.Domain.Models
{
    public class Order : ICaseRelated
    {
        public int Id { get; set; }
        public Offer Offer { get; set; }
        public Invoice Invoice { get; set; }
        public Customer Customer { get; set; }
        public Building Building { get; set; }
        public Contact Contact { get; set; }
        public VatRate VatRate { get; set; }
        public IEnumerable<Invoiceline> Invoicelines { get; set; }
        public IEnumerable<Deposit> Deposits { get; set; }
        public IEnumerable<DepositInvoice> DepositInvoices { get; set; }
        public IEnumerable<Intervention> Interventions { get; set; }

        public DateTime? OrderDate { get; set; }
        public int? RequestNumber { get; set; }
        public string OfferNumber { get; set; }
        public double? Amount { get; set; }
        public string Reference { get; set; }
        public bool DepositRequired { get; set; }
        public bool HasProductionTicket { get; set; }
        public bool MustBeInstalled { get; set; }
        public bool IsReady { get; set; }
        public bool MustBeDelivered { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public float? ScheduledHours { get; set; }
        public float? ScheduledNbOfPersons { get; set; }
        public string Comment { get; set; }
        public bool Canceled { get; set; }
        public string CancellationReason { get; set; }
        public DateTime? PlanningDate { get; set; }
        public string PlanningId { get; set; }
        public string PlanningMsObjectId { get; set; }
    }
}