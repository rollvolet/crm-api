namespace Rollvolet.CRM.APIContracts.DTO
{
    public class ContactDto : CustomerEntityDto
    {
        public override string Type { get; set; } = "contacts";
        public int RelativeId { get; set; }
        public int CustomerId { get; set; }
  }
}