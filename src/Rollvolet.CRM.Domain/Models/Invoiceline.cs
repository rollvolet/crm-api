namespace Rollvolet.CRM.Domain.Models
{
    public class Invoiceline
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public Invoice Invoice { get; set; }
        public VatRate VatRate { get; set; }

        public int SequenceNumber { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }
    }
}