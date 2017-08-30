namespace Rollvolet.CRM.Domain.Models
{
    public class Building : CustomerEntity
    {
        public int Id { get; set; }
        public int RelativeId { get; set; }
        public int CustomerId { get; set; }
    }
}