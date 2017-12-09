namespace Rollvolet.CRM.Domain.Models
{
    public class Telephone
    {
        public string Id { get; set; }
        public string Area { get; set; }
        public string Number { get; set; }
        public string Memo { get; set; }
        public int Order { get; set; }
        public Building Building { get; set; }        
        public Customer Customer { get; set; }
        public Contact Contact { get; set; }
        public Country Country { get; set; }
        public TelephoneType TelephoneType { get; set; }
    }
}