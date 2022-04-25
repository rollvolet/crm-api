using System;

namespace Rollvolet.CRM.APIContracts.DTO.Requests
{
    public class RequestAttributesDto
    {
        public DateTime? RequestDate { get; set; }
        public bool RequiresVisit { get; set; }
        public string Comment { get; set; }
        public string Employee { get; set; }
        public bool OfferExpected { get; set; }
        public string Visitor { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string CancellationReason { get; set; }
    }
}