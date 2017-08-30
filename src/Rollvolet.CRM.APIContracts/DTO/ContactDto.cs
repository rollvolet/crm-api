namespace Rollvolet.CRM.APIContracts.DTO
{
    public class ContactDto : CustomerEntityDto
    {
        public int RelativeId { get; set; }
        public int CustomerId { get; set; }
  }
}