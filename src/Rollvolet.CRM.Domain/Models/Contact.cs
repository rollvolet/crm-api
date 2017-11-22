namespace Rollvolet.CRM.Domain.Models
{
    public class Contact : CustomerEntity
    {
        public string Email { get; set; }
        public int CustomerId { get; set; }
    }
}