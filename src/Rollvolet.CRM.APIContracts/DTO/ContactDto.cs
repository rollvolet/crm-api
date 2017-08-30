namespace Rollvolet.CRM.APIContracts.DTO
{
    public class ContactDto
    {
        public string Id { get; set; }
        public string Type = "contacts";
        public int RelativeId { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
  }
}