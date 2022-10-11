using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Rollvolet.CRM.DataProvider.Models.Interfaces;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Invoice : ICaseRelated
    {
        [Column("FactuurId")]
        public int Id { get; set; }

        [Column("Jaar")]
        public short Year { get; set; }

        [Column("Nummer")]
        public int? Number { get; set; }

        [Column("Datum")]
        public DateTime? InvoiceDate { get; set; }

        [Column("VervalDag")]
        public DateTime? DueDate { get; set; }

        [Column("Geboekt")]
        public DateTime? BookingDate { get; set; }

        [Column("BetaalDatum")]
        public DateTime? PaymentDate { get; set; }

        [Column("Afgesloten")]
        public DateTime? CancellationDate { get; set; }

        [Column("KlantID")]
        public int? CustomerId { get; set; }

        [Column("GebouwID")]
        public int? RelativeBuildingId { get; set; }

        [Column("ContactID")]
        public int? RelativeContactId { get; set; }

        [Column("OfferteID")]
        public int? OrderId { get; set; }

        [Column("InterventionId")]
        public int? InterventionId { get; set; }

        [Column("BtwId")]
        public int? VatRateId { get; set; }

        [Column("BasisBedrag")]
        // in Access: amount copied from order for invoice
        // in RKB:
        // - manually entered by user for deposit invoice
        // - sum of invoicelines for regular invoice (as calculated by frontend)
        public double? BaseAmount { get; set; }

        // Amount, Vat and TotalAmount are kept in sync by InvoiceDataProvider.CalculateAmountAndVatAsync

        [Column("Bedrag")]
        public double? Amount { get; set; }  // baseAmount - all DepositInvoices

        [Column("BTWBedrag")]
        public double? Vat { get; set; } // VAT calculated on Amount

        [Column("Totaal")]
        public double? TotalAmount { get; set; } // gross price: amount + vat

        // @Deprecated
        // [Column("Kontant")]
        // public bool IsPaidInCash { get; set; }

        [Column("Attest")]
        public bool CertificateRequired { get; set; }

        [Column("AttestTerug")]
        public bool CertificateReceived { get; set; }

        // TODO what's the use of this field? wheter certificate may be reused on other invoices?
        [Column("AttestAfgesloten")]
        public bool CertificateClosed { get; set; }

        [Column("CreditNota")]
        public bool IsCreditNote { get; set; }

        [Column("Produktiebon")]
        public bool? HasProductionTicket { get; set; }

        // @Deprecated Contained the invoice number of the invoice who's VAT certificate is reused. Same certificate needs to be re-uploaded now
        // [Column("AttestLink")]
        // public string CertificateUrl { get; set; }

        [Column("Opmerking")] // kept in sync with order comment in frontend
        public string Comment { get; set; }

        // TODO what's the use of this field? How is it filled?
        [Column("Hoedanigheid")]
        public string Qualification { get; set; }

        [Column("Referentie")] // kept in sync with offer reference in frontend
        public string Reference { get; set; }

        [Column("DocumentOutro")]
        public string DocumentOutro { get; set; }

        [Column("MuntEenheid")]
        public string Currency { get; set; }

        [Column("Origin")]
        public string Origin { get; set; }


        // Include resources
        public Customer Customer { get; set; }
        public Building Building { get; set; }
        public Contact Contact { get; set; }
        public Order Order { get; set; }
        public Intervention Intervention { get; set; }
        public VatRate VatRate { get; set; }
        public IEnumerable<Deposit> Deposits { get; set; }
        public IEnumerable<DepositInvoiceHub> DepositInvoiceHubs { get; set; } // only set on normal invoices
        public DepositInvoiceHub MainInvoiceHub { get; set; } // only set on deposit invoices



        // Embedded properties

        // Embedded Customer properties

        [Column("KlantNaam")]
        public string CustomerName { get; set; }

        [Column("KlantAdres1")]
        public string CustomerAddress1 { get; set; }

        [Column("KlantAdres2")]
        public string CustomerAddress2 { get; set; }

        [Column("KlantAdres3")]
        public string CustomerAddress3 { get; set; }

        [Column("KlantPostcodeID")]
        public int? CustomerPostalCodeId { get; set; }

        [Column("KlantPostcode")]
        public string CustomerPostalCode { get; set; }

        [Column("KlantGemeente")]
        public string CustomerCity { get; set; }

        [Column("KlantTaalID")]
        public int? CustomerLanguageId { get; set; }

        [Column("KlantLandId")]
        public int? CustomerCountryId { get; set; }

        [Column("KlantAanspreekID")]
        public int? CustomerHonorificPrefixId { get; set; }

        [Column("KlantPrefix")]
        public string CustomerPrefix { get; set; }

        [Column("KlantSuffix")]
        public string CustomerSuffix { get; set; }

        [Column("KlantFirma")]
        public bool CustomerIsCompany { get; set; }

        [Column("KlantBTWNummer")]
        public string CustomerVatNumber { get; set; }

        [Column("KlantTel")]
        public string CustomerPhoneNumber { get; set; }

        [Column("KlantGSM")]
        public string CustomerMobileNumber { get; set; }

        [Column("KlantFax")]
        public string CustomerFaxNumber { get; set; }

        [Column("KlantZoekNaam")]
        public string CustomerSearchName { get; set; }

        // Embedded Building properties

        [Column("GebouwNaam")]
        public string BuildingName { get; set; }

        [Column("GebouwAdres1")]
        public string BuildingAddress1 { get; set; }

        [Column("GebouwAdres2")]
        public string BuildingAddress2 { get; set; }

        [Column("GebouwAdres3")]
        public string BuildingAddress3 { get; set; }

        [Column("GebouwPostcodeID")]
        public int? BuildingPostalCodeId { get; set; }

        [Column("GebouwPostcode")]
        public string BuildingPostalCode { get; set; }

        [Column("GebouwGemeente")]
        public string BuildingCity { get; set; }

        [Column("GebouwLandId")]
        public int? BuildingCountryId { get; set; }

        [Column("GebouwPrefix")]
        public string BuildingPrefix { get; set; }

        [Column("GebouwSuffix")]
        public string BuildingSuffix { get; set; }

        [Column("GebouwTel")]
        public string BuildingPhoneNumber { get; set; }

        [Column("GebouwGSM")]
        public string BuildingMobileNumber { get; set; }

        [Column("GebouwFax")]
        public string BuildingFaxNumber { get; set; }

        [Column("GebouwZoekNaam")]
        public string BuildingSearchName { get; set; }

        // Embedded Contact properties

        [Column("ContactNaam")]
        public string ContactName { get; set; }

        [Column("ContactAdres1")]
        public string ContactAddress1 { get; set; }

        [Column("ContactAdres2")]
        public string ContactAddress2 { get; set; }

        [Column("ContactAdres3")]
        public string ContactAddress3 { get; set; }

        [Column("ContactPostcodeID")]
        public int? ContactPostalCodeId { get; set; }

        [Column("ContactPostcode")]
        public string ContactPostalCode { get; set; }

        [Column("ContactGemeente")]
        public string ContactCity { get; set; }

        [Column("ContactLandId")]
        public int? ContactCountryId { get; set; }

        [Column("ContactTaalID")]
        public int? ContactLanguageId { get; set; }

        [Column("ContactAanspreekID")]
        public int? ContactHonorificPrefixId { get; set; }

        [Column("ContactPrefix")]
        public string ContactPrefix { get; set; }

        [Column("ContactSuffix")]
        public string ContactSuffix { get; set; }

        [Column("ContactTel")]
        public string ContactPhoneNumber { get; set; }

        [Column("ContactGSM")]
        public string ContactMobileNumber { get; set; }

        [Column("ContactFax")]
        public string ContactFaxNumber { get; set; }

        [Column("ContactZoekNaam")]
        public string ContactSearchName { get; set; }

    }
}
