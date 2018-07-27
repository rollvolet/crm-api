using System;
using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.Deposits
{
    public class DepositAttributesDto
    {
        [JsonProperty("sequence-number")]
        public int SequenceNumber { get; set; }
        public double? Amount { get; set; }
        [JsonProperty("payment-date")]
        public DateTime PaymentDate { get; set; }
    }
}