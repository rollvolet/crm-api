namespace Rollvolet.CRM.APIContracts.DTO
{
    public class CountryDto
    {
        public string Id { get; set; }
        public string Type { get; set; } = "countries";
        public string Code { get; set; }
        public string Name { get; set; }
  }
}