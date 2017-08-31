using Rollvolet.CRM.APIContracts.JsonApi.Interfaces;

namespace Rollvolet.CRM.APIContracts.DTO
{
    public abstract class CustomerEntityDto : IDto
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public CountryDto Country { get; set; }
  }
}