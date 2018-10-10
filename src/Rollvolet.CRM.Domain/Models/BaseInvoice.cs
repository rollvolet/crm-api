using System;
using System.Collections.Generic;

namespace Rollvolet.CRM.Domain.Models
{
    public class BaseInvoice
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public Customer Customer { get; set; }
        public Building Building { get; set; }
        public Contact Contact { get; set; }
        public Order Order { get; set; }
        public VatRate VatRate { get; set; }
        public double? BaseAmount { get; set; }
        public bool IsPaidInCash { get; set; }
        public bool CertificateRequired { get; set; }
        public bool CertificateReceived { get; set; }
        public bool CertificateClosed { get; set; }
        public bool IsCreditNote { get; set; }
        public bool HasProductionTicket { get; set; }
        public string CertificateUrl { get; set; }
        public string Comment { get; set; }
        public string Qualification { get; set; }
        public string Reference { get; set; }


        // Embedded Customer properties
        public string CustomerName { get; set; }
        public string CustomerAddress1 { get; set; }
        public string CustomerAddress2 { get; set; }
        public string CustomerAddress3 { get; set; }
        public int? CustomerPostalCodeId { get; set; }
        public string CustomerPostalCode { get; set; }
        public string CustomerCity { get; set; }
        public int? CustomerLanguageId { get; set; }
        public int? CustomerCountryId { get; set; }
        public int? CustomerHonorificPrefixId { get; set; }
        public string CustomerPrefix { get; set; }
        public string CustomerSuffix { get; set; }
        public bool CustomerIsCompany { get; set; }
        public string CustomerVatNumber { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerMobileNumber { get; set; }
        public string CustomerFaxNumber { get; set; }
        public string CustomerSearchName { get; set; }

        // Embedded Building properties
        public string BuildingName { get; set; }
        public string BuildingAddress1 { get; set; }
        public string BuildingAddress2 { get; set; }
        public string BuildingAddress3 { get; set; }
        public int? BuildingPostalCodeId { get; set; }
        public string BuildingPostalCode { get; set; }
        public string BuildingCity { get; set; }
        public int? BuildingCountryId { get; set; }
        public string BuildingPrefix { get; set; }
        public string BuildingSuffix { get; set; }
        public string BuildingPhoneNumber { get; set; }
        public string BuildingMobileNumber { get; set; }
        public string BuildingFaxNumber { get; set; }
        public string BuildingSearchName { get; set; }

        // Embedded Contact properties
        public string ContactName { get; set; }
        public string ContactAddress1 { get; set; }
        public string ContactAddress2 { get; set; }
        public string ContactAddress3 { get; set; }
        public int? ContactPostalCodeId { get; set; }
        public string ContactPostalCode { get; set; }
        public string ContactCity { get; set; }
        public int? ContactCountryId { get; set; }
        public int? ContactLanguageId { get; set; }
        public int? ContactHonorificPrefixId { get; set; }
        public string ContactPrefix { get; set; }
        public string ContactSuffix { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string ContactMobileNumber { get; set; }
        public string ContactFaxNumber { get; set; }
        public string ContactSearchName { get; set; }
    }
}