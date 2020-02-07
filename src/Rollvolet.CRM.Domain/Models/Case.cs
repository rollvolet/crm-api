namespace Rollvolet.CRM.Domain.Models
{
    public class Case
    {
        public int? CustomerId { get; set; }
        public int? ContactId { get; set; }
        public int? BuildingId { get; set; }
        public int? RequestId { get; set; }
        public int? OfferId { get; set; }
        public int? OrderId { get; set; }
        public int? InvoiceId { get; set; }
    }
}