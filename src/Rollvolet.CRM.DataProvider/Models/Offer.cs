using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Rollvolet.CRM.DataProvider.Models.Interfaces;

namespace Rollvolet.CRM.DataProvider.Models
{
    public class Offer : ICaseRelated
    {
        [Column("OfferteID")]
        public int Id { get; set; }

        [Column("AanvraagId")]
        public int? RequestId { get; set; }

        [Column("KlantID")]
        public int? CustomerId { get; set; }

        [Column("GebouwID")]
        public int? RelativeBuildingId { get; set; }

        [Column("ContactID")]
        public int? RelativeContactId { get; set; }

        [Column("OfferteNr")]
        public string Number { get; set; }

        [Column("VolgNummer")]
        public short SequenceNumber { get; set; }

        // offerDate reflects the last time the order document is generated
        // this doesn't need to be in sync with the date included in the offernumber
        [Column("Offertedatum")]
        public DateTime? OfferDate { get; set; }

        [Column("BtwId")]
        public int? VatRateId { get; set; }

        // @Deprecated
        // [Column("VerzondenVia")]
        // public string SubmissionTypeId { get; set; }

        [Column("Referentie")] // kept in sync with invoice reference in frontend
        public string Reference { get; set; }

        [Column("DocumentIntro")]
        public string DocumentIntro { get; set; }

        [Column("DocumentOutro")]
        public string DocumentOutro { get; set; }

        [Column("DocumentVersion")]
        public string DocumentVersion { get; set; }

        [Column("MuntOfferte")]
        public string Currency { get; set; }

        // @Deprecated Needs to be moved to offerline
        // [Column("UrenVoorzien")]
        // public float? ForeseenHours { get; set; }

        // @Deprecated Needs to be moved to offerline
        // [Column("ManVoorzien")]
        // public float? ForeseenNbOfPersons { get; set; }

        // Only set on offers created in Access
        // For new offers the amount is calculated based on the sum of the offerlines
        // TODO @Deprecated once Access is not used anymore
        //      or maybe recalculate and persist in DB for reporting purposes?
        [Column("OfferteBedrag")]
        public double? Amount { get; set; }

        [Column("Opmerking")] // kept in sync with invoice comment in frontend
        public string Comment { get; set; }

        // @Deprecated
        // [Column("VerzendDatum")]

        // @Deprecated
        // [Column("Afgesloten")]

        // @Deprecated
        // [Column("ProduktId")]


        // Include resources
        public Request Request { get; set; }
        public Customer Customer { get; set; }
        public Building Building { get; set; }
        public Contact Contact { get; set; }
        public VatRate VatRate { get; set; }
        public Order Order { get; set; }


        // Embedded properties
        [Column("Gemeente")]
        public string EmbeddedCity { get; set; }

    }
}