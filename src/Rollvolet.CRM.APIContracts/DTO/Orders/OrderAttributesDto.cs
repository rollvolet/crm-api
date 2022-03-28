using System;

namespace Rollvolet.CRM.APIContracts.DTO.Orders
{
    public class OrderAttributesDto
    {
        public DateTime? OrderDate { get; set; }
        public string OfferNumber { get; set; }
        public double? Amount { get; set; }
        public string Reference { get; set; }
        public int? RequestNumber { get; set; }
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
    }
}