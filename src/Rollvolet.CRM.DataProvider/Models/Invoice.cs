using System;
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
        public int Number { get; set; }

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

        [Column("BtwId")]
        public int? VatRateId { get; set; }

        [Column("BasisBedrag")]
        public double? BaseAmount { get; set; }     

        [Column("Bedrag")]
        public double? Amount { get; set; } 

        [Column("BTWBedrag")]
        public double? Vat { get; set; }   

        [Column("Totaal")]
        public double? TotalAmount { get; set; }   

        [Column("Kontant")]      
        public bool IsPaidInCash { get; set; }  

        [Column("Attest")]      
        public bool CertificateRequired { get; set; }  

        [Column("AttestTerug")]      
        public bool CertificateReceived { get; set; }  

        [Column("AttestAfgesloten")]      
        public bool CertificateClosed { get; set; }  

        [Column("CreditNota")]      
        public bool IsCreditNote { get; set; }  

        [Column("Produktiebon")]
        public bool? HasProductionTicket { get; set; }

        [Column("UrenGewerkt")]
        public float? PerformedHours { get; set; }

        [Column("ManGewerkt")]
        public float? PerformedNbOfPersons { get; set; }

        [Column("AttestLink")]
        public string CertificateUrl { get; set; } 

        [Column("Opmerking")]
        public string Comment { get; set; } 

        [Column("Hoedanigheid")]
        public string Qualification { get; set; } 

        [Column("Referentie")]
        public string Reference { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("UpdTimestamp")]
        public DateTime Updated { get; set; }

        [Column("MuntEenheid")]
        public string Currency { get; set; }


        // Include resources
        public Customer Customer { get; set; }
        public Order Order { get; set; }
        public VatRate VatRate { get; set; }
        

        // Manually included resources
        [NotMapped]
        public Building Building { get; set; }
        [NotMapped]
        public Contact Contact { get; set; }



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