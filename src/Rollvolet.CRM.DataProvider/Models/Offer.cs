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

        [Column("Offertedatum")]
        public DateTime? OfferDate { get; set; }

        [Column("BtwId")]
        public int? VatRateId { get; set; }

        [Column("VerzondenVia")]
        public string SubmissionTypeId { get; set; }

        [Column("UrenVoorzien")]
        public float? ForeseenHours { get; set; }

        [Column("ManVoorzien")]
        public float? ForeseenNbOfPersons { get; set; }

        [Column("Opmerking")]
        public string Comment { get; set; }

        [Column("Referentie")]
        public string Reference { get; set; }

        [Column("DocumentIntro")]
        public string DocumentIntro { get; set; }

        [Column("DocumentOutro")]
        public string DocumentOutro { get; set; }

        [Column("MuntOfferte")]
        public string Currency { get; set; }

        // DEPRECATED
        // [Column("OfferteBedrag")]
        // public double? Amount { get; set; }

        // DEPRECATED
        // [Column("VerzendDatum")]

        // DEPRECATED
        // [Column("Afgesloten")]

        // DEPRECATED
        // [Column("ProduktId")]



        // Include resources
        public Request Request { get; set; }
        public Customer Customer { get; set; }
        public VatRate VatRate { get; set; }
        public SubmissionType SubmissionType { get; set; }
        public Order Order { get; set; }
        public IEnumerable<Offerline> Offerlines { get; set; }


        // Manually included resources
        [NotMapped]
        public Building Building { get; set; }
        [NotMapped]
        public Contact Contact { get; set; }


        // Embedded properties
        [Column("Gemeente")]
        public string EmbeddedCity { get; set; }

    }
}