using System;

namespace Rollvolet.CRM.APIContracts.DTO.Reports
{
    public class OutstandingJobAttributesDto
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
        public DateTime? ExpectedDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? PlanningDate { get; set; }
        public bool HasProductionTicket { get; set; }
        public bool ProductIsReady { get; set; }
        public bool MustBeDelivered { get; set; }
        public bool MustBeInstalled { get; set; }
        public string Comment { get; set; }
        public string Technicians { get; set; }
    }
}