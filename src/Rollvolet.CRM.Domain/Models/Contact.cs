namespace Rollvolet.CRM.Domain.Models
{
    public class Contact : CustomerEntity
    {
        public Customer Customer { get; set; }
    }
}