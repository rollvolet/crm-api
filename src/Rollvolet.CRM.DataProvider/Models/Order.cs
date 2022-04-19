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

        [Column("AanvraagId")]
        public int? RequestId { get; set; } // Only for performance reasons to search by RequestId quickly

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

        // Only set on orders created in Access
        // For new order the amount is calculated based on the sum of the ordered offerlines
        // TODO @Deprecated once Access is not used anymore
        //      or maybe recalculate and persist in DB for reporting purposes?
        // @Deprecated
        [Column("BestelTotaal")]
        public double? Amount { get; set; }

        [Column("Referentie")] // kept in sync with invoice reference in frontend
        public string Reference { get; set; }

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

        // TODO convert to DateTime
        [Column("VerwachteDatum")]
        public string ExpectedDate { get; set; }

        // TODO convert to DateTime
        [Column("VereisteDatum")]
        public string RequiredDate { get; set; }

        [Column("UrenGepland")]
        public float? ScheduledHours { get; set; }

        [Column("ManGepland")]
        public float? ScheduledNbOfPersons { get; set; }

        [Column("Opmerking")] // kept in sync with invoice comment in frontend
        public string Comment { get; set; }

        [Column("AfgeslotenBestelling")]
        public bool Canceled { get; set; }

        [Column("RedenAfsluiten")]
        public string CancellationReason { get; set; }

        [Column("MuntBestel")]
        public string Currency { get; set; }

        [Column("Besteld")]
        public bool IsOrdered { get; set; }

        // TODO convert to DateTime
        // Kept in sync with calendar-event date by frontend.
        // Still required in SQL DB for reporting purposes.
        // Can be removed once orders are converted to triplestore.
        [Column("VastgelegdeDatum")]
        public string PlanningDate { get; set; }

        // @Deprecated
        // [Column("UrenGerekend")]
        // public float? InvoicableHours { get; set; }

        // @Deprecated
        // [Column("ManGerekend")]
        // public float? InvoicableNbOfPersons { get; set; }

        // @Deprecated  Not used anymore due to addition of InvoiceSupplements
        // [Column("Bedrag")]
        // public float? BaseAmount

        // Include resources
        public Offer Offer { get; set; }
        public Invoice Invoice { get; set; }
        public Customer Customer { get; set; }
        public Building Building { get; set; }
        public Contact Contact { get; set; }
        public VatRate VatRate { get; set; }
        public IEnumerable<Deposit> Deposits { get; set; }
        public IEnumerable<DepositInvoiceHub> DepositInvoicesHubs { get; set; }
        public IEnumerable<Intervention> Interventions { get; set; }

        public ICollection<OrderTechnician> OrderTechnicians { get; set; }
    }
}
