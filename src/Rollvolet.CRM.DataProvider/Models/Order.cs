using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Rollvolet.CRM.DataProvider.Models.Interfaces;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Order : ICaseRelated
    {
        [Column("OfferteID")]
        public int Id { get; set; }  // Offer and Order have the same ID column
        
        [Column("KlantID")]
        public int? CustomerId { get; set; }
        
        [Column("GebouwID")]
        public int? RelativeBuildingId { get; set; }
        
        [Column("ContactID")]
        public int? RelativeContactId { get; set; }
        
        [Column("BestelDatum")]
        public DateTime? OrderDate { get; set; }

        [Column("OfferteNr")]
        public string OfferNumber { get; set; }

        [Column("BestelTotaal")]
        public double? Amount { get; set; }

        [Column("BtwId")]
        public int? VatRateId { get; set; }

        [Column("VoorschotNodig")]
        public bool DepositRequired { get; set; }

        [Column("Produktiebon")]
        public bool HasProductionTicket { get; set; }

        [Column("Plaatsing")]
        public bool MustBeInstalled { get; set; }

        [Column("ProductKlaar")]
        public bool IsReady { get; set; }

        [Column("TeLeveren")]
        public bool MustBeDelivered { get; set; }
        
        [Column("VerwachteDatum")]
        public string ExpectedDate { get; set; }
        
        [Column("VereisteDatum")]
        public string RequiredDate { get; set; }

        [Column("UrenGepland")]
        public float? ScheduledHours { get; set; }

        [Column("ManGepland")]
        public float? ScheduledNbOfPersons { get; set; }  

        [Column("UrenGerekend")]
        public float? InvoicableHours { get; set; }

        [Column("ManGerekend")]
        public float? InvoicableNbOfPersons { get; set; }  

        [Column("Opmerking")]
        public string Comment { get; set; }                 

        [Column("AfgeslotenBestelling")]
        public bool Canceled { get; set; }

        [Column("RedenAfsluiten")]
        public string CancellationReason { get; set; } 

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("UpdTimestampBST")]
        public DateTime Updated { get; set; }

        [Column("MuntBestel")]
        public string Currency { get; set; }

        [Column("Besteld")]      
        public bool IsOrdered { get; set; }  


        // Include resources
        public Offer Offer { get; set; }
        public Customer Customer { get; set; }
        public VatRate VatRate { get; set; }
        public IEnumerable<Deposit> Deposits { get; set; }

        // TODO add calendar event
        

        // Manually included resources
        [NotMapped]
        public Building Building { get; set; }
        [NotMapped]
        public Contact Contact { get; set; }        
    }
}