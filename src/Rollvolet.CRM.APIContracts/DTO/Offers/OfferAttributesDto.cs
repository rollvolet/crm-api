using System;

namespace Rollvolet.CRM.APIContracts.DTO.Offers
{
    public class OfferAttributesDto
    {
        public string Number { get; set; }
        public int SequenceNumber { get; set; }
        public int? RequestNumber { get; set; }
        public DateTime OfferDate { get; set; }
        public double? Amount { get; set; }
        public string Reference { get; set; }
        public string Comment { get; set; }
        public string DocumentIntro { get; set; }
        public string DocumentOutro { get; set; }
        public string DocumentVersion { get; set; }
    }
}