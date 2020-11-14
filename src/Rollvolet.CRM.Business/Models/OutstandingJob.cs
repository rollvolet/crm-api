using System;

namespace Rollvolet.CRM.Business.Models
{
    public class OutstandingJob
    {
        public string OrderId { get; set; }
        public string OfferNumber { get; set; }
        public string RequestId { get; set; }
        public string Visitor { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress1 { get; set; }
        public string CustomerAddress2 { get; set; }
        public string CustomerAddress3 { get; set; }
        public string CustomerPostalCode { get; set; }
        public string CustomerCity { get; set; }
        public string BuildingName { get; set; }
        public string BuildingAddress1 { get; set; }
        public string BuildingAddress2 { get; set; }
        public string BuildingAddress3 { get; set; }
        public string BuildingPostalCode { get; set; }
        public string BuildingCity { get; set; }
        public float? ScheduledNbOfHours { get; set; }
        public float? ScheduledNbOfPersons { get; set; }
        // TODO convert to DateTime
        public string ExpectedDate { get; set; }
        // TODO convert to DateTime
        public string RequiredDate { get; set; }
        // TODO convert to DateTime
        public string PlanningDate { get; set; }
        public bool HasProductionTicket { get; set; }
        public bool ProductIsReady { get; set; }
        public bool MustBeDelivered { get; set; }
        public bool MustBeInstalled { get; set; }
    }
}
