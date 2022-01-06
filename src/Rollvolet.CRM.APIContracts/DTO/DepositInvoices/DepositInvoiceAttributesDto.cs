using System;

namespace Rollvolet.CRM.APIContracts.DTO.DepositInvoices
{
    public class DepositInvoiceAttributesDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public double? BaseAmount { get; set; }
        public double? Vat { get; set; }
        public bool CertificateRequired { get; set; }
        public bool CertificateReceived { get; set; }
        public bool CertificateClosed { get; set; }
        public bool IsCreditNote { get; set; }
        public string Comment { get; set; }
        public string Qualification { get; set; }
        public string DocumentOutro { get; set; }
        public string Reference { get; set; }
        public string Origin { get; set; }
    }
}
