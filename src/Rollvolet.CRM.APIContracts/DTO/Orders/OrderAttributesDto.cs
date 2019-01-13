using System;
using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.Orders
{
    public class OrderAttributesDto
    {
        [JsonProperty("order-date")]
        public DateTime? OrderDate { get; set; }
        [JsonProperty("offer-number")]
        public string OfferNumber { get; set; }
        public double? Amount { get; set; }
        [JsonProperty("request-number")]
        public int? RequestNumber { get; set; }
        [JsonProperty("deposit-required")]
        public bool DepositRequired { get; set; }
        [JsonProperty("has-production-ticket")]
        public bool HasProductionTicket { get; set; }
        [JsonProperty("must-be-installed")]
        public bool MustBeInstalled { get; set; }
        [JsonProperty("is-ready")]
        public bool IsReady { get; set; }
        [JsonProperty("must-be-delivered")]
        public bool MustBeDelivered { get; set; }
        [JsonProperty("expected-date")]
        public DateTime? ExpectedDate { get; set; }
        [JsonProperty("required-date")]
        public DateTime? RequiredDate { get; set; }
        [JsonProperty("scheduled-hours")]
        public float? ScheduledHours { get; set; }
        [JsonProperty("scheduled-nb-of-persons")]
        public float? ScheduledNbOfPersons { get; set; }
        public string Comment { get; set; }
        public bool Canceled { get; set; }
        [JsonProperty("cancellation-reason")]
        public string CancellationReason { get; set; }
        [JsonProperty("planning-date")]
        public DateTime? PlanningDate { get; set; }
        [JsonProperty("planning-id")]
        public string PlanningId { get; set; }
        [JsonProperty("planning-ms-object-id")]
        public string PlanningMsObjectId { get; set; }
    }
}