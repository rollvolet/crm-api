namespace Rollvolet.CRM.Domain.Models
{
    public abstract class CustomerEntity
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public Country Country { get; set; }
    }
}