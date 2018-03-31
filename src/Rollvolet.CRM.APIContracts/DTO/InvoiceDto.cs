using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Rollvolet.CRM.APIContracts.JsonApi;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public class InvoiceDto : Resource<InvoiceDto.AttributesDto, InvoiceDto.RelationshipsDto>
    {
        public class AttributesDto
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
            public double? Amount { get; set; } 
            public double? Vat { get; set; }   
            [JsonProperty("total-amount")]
            public double? TotalAmount { get; set; }   
            [JsonProperty("is-paid-in-cash")]
            public bool IsPaidInCash { get; set; }  
            [JsonProperty("certificate-required")]
            public bool CertificateRequired { get; set; }  
            [JsonProperty("certificate-received")]
            public bool CertificateReceived { get; set; }  
            [JsonProperty("certificate-closed")]
            public bool CertificateClosed { get; set; }  
            [JsonProperty("is-credit-note")]
            public bool IsCreditNote { get; set; }  
            [JsonProperty("has-production-ticket")]
            public bool HasProductionTicket { get; set; }
            [JsonProperty("performed-hours")]
            public float? PerformedHours { get; set; }
            [JsonProperty("performed-nb-of-persons")]
            public float? PerformedNbOfPersons { get; set; }
            [JsonProperty("certificate-url")]
            public string CertificateUrl { get; set; } 
            public string Comment { get; set; } 
            public string Qualification { get; set; } 
            public string Reference { get; set; }
        }

        public class RelationshipsDto
        {
            public IRelationship Order { get; set; }
            public IRelationship Customer { get; set; }
            public IRelationship Building { get; set; }
            public IRelationship Contact { get; set; }
            [JsonProperty("vat-rate")]
            public IRelationship VatRate { get; set; }
            public IRelationship Supplements { get; set; }
            public IRelationship Deposits { get; set; }
            [JsonProperty("deposit-invoices")]
            public IRelationship DepositInvoices { get; set; }
            public IRelationship WorkingHours { get; set; }
        }
    }
}