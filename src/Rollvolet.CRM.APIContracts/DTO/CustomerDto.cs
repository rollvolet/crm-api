namespace Rollvolet.CRM.APIContracts.DTO
{
    public class CustomerDto
    {
        public string Id { get; set; }
        public string Type = "customers";
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
  }
}