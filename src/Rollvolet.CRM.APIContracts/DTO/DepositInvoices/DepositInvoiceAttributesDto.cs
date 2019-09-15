using System;
using Newtonsoft.Json;

namespace Rollvolet.CRM.APIContracts.DTO.DepositInvoices
{
    public class DepositInvoiceAttributesDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        [JsonProperty("invoice-date")]
        public DateTime? InvoiceDate { get; set; }
        [JsonProperty("due-date")]
        public DateTime? DueDate { get; set; }
        [JsonProperty("booking-date")]
        public DateTime? BookingDate { get; set; }
        [JsonProperty("payment-date")]
        public DateTime? PaymentDate { get; set; }
        [JsonProperty("cancellation-date")]
        public DateTime? CancellationDate { get; set; }
        [JsonProperty("base-amount")]
        public double? BaseAmount { get; set; }
        public double? Vat { get; set; }
        [JsonProperty("certificate-required")]
        public bool CertificateRequired { get; set; }
        [JsonProperty("certificate-received")]
        public bool CertificateReceived { get; set; }
        [JsonProperty("certificate-closed")]
        public bool CertificateClosed { get; set; }
        public string Comment { get; set; }
        public string Qualification { get; set; }
        [JsonProperty("document-outro")]
        public string DocumentOutro { get; set; }
        public string Reference { get; set; }
    }
}
