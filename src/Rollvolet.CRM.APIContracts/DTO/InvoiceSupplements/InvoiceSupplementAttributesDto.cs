using System;
using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.InvoiceSupplements
{
    public class InvoiceSupplementAttributesDto
    {
        public int Id { get; set; }
        [JsonProperty("sequence-number")]
        public int SequenceNumber { get; set; }
        [JsonProperty("nb-of-pieces")]
        public double? NbOfPieces { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }
    }
}
